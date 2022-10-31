using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;
using TelegramModularFramework.Services.TypeReaders;
using TelegramModularFramework.Services.Utils;

namespace TelegramModularFramework.Services;

public class TelegramModulesService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TelegramModulesService> _logger;
    private readonly TelegramBotHostedService _host;
    private readonly IStringSplitter _splitter;
    private readonly ITelegramBotClient _client;

    private List<ModuleInfo> _modules = new();
    public ImmutableArray<ModuleInfo> Modules => ImmutableArray.Create(_modules.ToArray());

    private Dictionary<string, CommandInfo> _commands = new();
    public Dictionary<string, CommandInfo> Commands => _commands;
    public IEnumerable<CommandInfo> VisibleCommands => _commands.Values.Where(c => !c.HiddenFromList);

    public event Func<CommandInfo?, ModuleContext, Result, Task> CommandExecuted; 

    public TelegramModulesService(IServiceProvider provider, ILogger<TelegramModulesService> logger,
        TelegramBotHostedService host, IStringSplitter splitter, ITelegramBotClient client)
    {
        _provider = provider;
        _logger = logger;
        _host = host;
        _splitter = splitter;
        _client = client;
    }

    public async Task<bool> HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        switch (update.Type)
        {
            case UpdateType.Message:
                await HandleCommand(botClient, update, cancellationToken);
                return true;
            default:
                return false;
        }
    }

    private async Task HandleCommand(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var args = update.Message.Text!.Split(' ');
        var commandString = args[0].Replace($"@{_host.User.Username}", "");
        args = args.Skip(1).ToArray();
        var context = new ModuleContext(botClient, update, args, commandString);

        if (_commands.TryGetValue(commandString, out var commandInfo))
        {
            _logger.LogDebug("Executing {command} from {module}", commandInfo.Name, commandInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = commandInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                module.Context = context;

                // Method
                Result result;
                try
                {
                    await InvokeCommand(commandInfo, module, scope.ServiceProvider, args);
                    result = Result.FromSuccess();
                }
                catch (BaseCommandException e)
                {
                    result = Result.FromError(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during executing {command} from {module}", commandInfo.Name, commandInfo.Module.Type.Name);
                    result = Result.FromError(e);
                }
                await CommandExecuted?.InvokeAsync(commandInfo, context, result);
            }
        }
        else
        {
            await CommandExecuted?.InvokeAsync(null, context, Result.FromError(new UnknownCommand()));
        }
    }

    private async Task InvokeCommand(CommandInfo commandInfo, BaseTelegramModule module, IServiceProvider scoped, string[] args)
    {
        var returnTask = commandInfo.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task));

        var parametersInfos = commandInfo.MethodInfo.GetParameters();
        var splittedArgs = _splitter.Split(String.Join(' ', args));
        var parameters = new object[parametersInfos.Length];
        
        for (int i = 0; i < parametersInfos.Length; i++)
        {
            var type = parametersInfos[i].ParameterType;
            
            if (parametersInfos[i].HasDefaultValue)
            {
                if (splittedArgs.Count < i + 1)
                {
                    parameters[i] = parametersInfos[i].DefaultValue;   
                    continue;
                }
            } 
            
            var underlyingType = Nullable.GetUnderlyingType(type);
            if (underlyingType != null)
            {
                if (splittedArgs.Count < i + 1)
                {
                    parameters[i] = null;   
                    continue;
                }
                type = underlyingType;
            } 
            
            if (splittedArgs.Count < i + 1)
            {
                throw new BadArgs(i);
            }
            
            var reader = _provider
                .GetServices<ITypeReader>()
                .First(r => r.Type == type);
            var read = await reader.ReadTypeAsync(module.Context, splittedArgs[i]);
            if (!read.Success)
            {
                throw new TypeConvertException(read.ErrorReason, parametersInfos[i], i);
            }
            parameters[i] = read.Result;
        }

        var result = commandInfo.MethodInfo.Invoke(module, parameters);

        if (returnTask && commandInfo.RunMode == RunMode.Sync)
            await (result as Task);
    }

    public void AddModule<T>() where T : BaseTelegramModule
    {
        AddModuleInternal(typeof(T));
    }

    public void AddModule(Type module)
    {
        if (!module.IsPublic || module.IsAbstract || !module.IsSubclassOf(typeof(BaseTelegramModule)))
        {
            throw new Exception($"Wrong module {module}");
        }

        AddModuleInternal(module);
    }

    private void AddModuleInternal(Type module)
    {
        var moduleInfo = new ModuleInfo()
        {
            Type = module,
            Factory = ActivatorUtilities.CreateFactory(module, new Type[] { })
        };

        var commands = module
            .GetMethods()
            .Where(m => m.IsDefined(typeof(CommandAttribute)))
            .Select(m => new CommandInfo()
            {
                MethodInfo = m,
                Attributes = m.GetCustomAttribute<CommandAttribute>(),
                Module = moduleInfo,
                Name = m.GetCustomAttribute<CommandAttribute>().Name ?? m.Name.ToLower(),
                Summary = m.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "",
                HiddenFromList = m.GetCustomAttribute<CommandAttribute>().HideFromList,
                RunMode = m.GetCustomAttribute<RunModeAttribute>()?.RunMode ?? RunMode.Sync
            });

        _modules.Add(moduleInfo);
        foreach (var command in commands)
        {
            if (!_commands.TryAdd("/" + command.Name, command))
            {
                throw new Exception($"Commands with name {command.Name} already exists");
            }
        }
    }

    public void AddModules(Assembly assembly = null)
    {
        if (assembly == null) assembly = Assembly.GetCallingAssembly();

        var modules = assembly
            .GetTypes()
            .Where(t => t.IsPublic && !t.IsAbstract && t.IsSubclassOf(typeof(BaseTelegramModule)));

        foreach (var module in modules)
        {
            AddModuleInternal(module);
        }
    }

    public async Task SetMyCommands()
    {
        var commands = _commands
            .Where(c => !c.Value.HiddenFromList && !string.IsNullOrEmpty(c.Value.Summary))
            .Select(c => new BotCommand()
            {
                Command = c.Value.Name,
                Description = c.Value.Summary
            });
        await _client.SetMyCommandsAsync(commands);
    }
}