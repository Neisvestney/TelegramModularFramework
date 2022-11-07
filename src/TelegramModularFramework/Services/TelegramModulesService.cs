using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramModularFramework.Attributes;
using TelegramModularFramework.Modules;
using TelegramModularFramework.Services.Exceptions;
using TelegramModularFramework.Services.State;
using TelegramModularFramework.Services.TypeReaders;
using TelegramModularFramework.Services.Utils;

namespace TelegramModularFramework.Services;

/// <summary>
/// Service for handling commands, actions and states from registered modules
/// </summary>
public class TelegramModulesService
{
    private readonly IServiceProvider _provider;
    private readonly ILogger<TelegramModulesService> _logger;
    private readonly TelegramBotHostedService _host;
    private readonly IStringSplitter _splitter;
    private readonly ITelegramBotClient _client;
    private readonly TelegramModulesConfiguration _config;
    private readonly IStateHolder _stateHolder;

    private List<ModuleInfo> _modules = new();
    
    /// <summary>
    /// List of all loaded modules
    /// </summary>
    public ImmutableArray<ModuleInfo> Modules => ImmutableArray.Create(_modules.ToArray());

    private Dictionary<string, CommandInfo> _commands = new();
    
    /// <summary>
    /// List of all commands
    /// </summary>
    public Dictionary<string, CommandInfo> Commands => _commands;
    
    /// <summary>
    /// List of all visible commands
    /// </summary>
    public IEnumerable<CommandInfo> VisibleCommands => _commands.Values.Where(c => !c.HiddenFromList);

    private Dictionary<string, ActionInfo> _actions = new();

    private Dictionary<string, StateInfo> _states = new();
    
    /// <summary>
    /// Async event that executes after command executed successfully or not
    /// </summary>
    public event Func<CommandInfo?, ModuleContext, Result, Task> CommandExecuted;
    
    /// <summary>
    /// Async event that executes after action executed successfully or not
    /// </summary>
    public event Func<ActionInfo?, ModuleContext, Result, Task> ActionExecuted;
    
    /// <summary>
    /// Async event that executes after state executed successfully or not
    /// </summary>
    public event Func<StateInfo?, ModuleContext, Result, Task> StateExecuted;

    public TelegramModulesService(IServiceProvider provider, ILogger<TelegramModulesService> logger,
        TelegramBotHostedService host, IStringSplitter splitter, ITelegramBotClient client,
        IOptions<TelegramModulesConfiguration> config, IStateHolder stateHolder)
    {
        _provider = provider;
        _logger = logger;
        _host = host;
        _splitter = splitter;
        _client = client;
        _config = config.Value;
        _stateHolder = stateHolder;
    }

    /// <summary>
    /// Handle update from telegram bot api and executes modules
    /// </summary>
    /// <param name="botClient">BotClient from update event</param>
    /// <param name="update">Update object from update event</param>
    /// <param name="cancellationToken">CancellationToken from update event</param>
    /// <returns>true if update handled by framework otherwise returns false</returns>
    public async Task<bool> HandleUpdateAsync(ITelegramBotClient botClient, Update update,
        CancellationToken cancellationToken)
    {
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    var state = await _stateHolder.GetState(update.Message.Chat.Id);
                    if (!string.IsNullOrEmpty(state) && state != "/")
                    {
                        await HandleState(botClient, update, state, cancellationToken);
                        return true;
                    }
                    else if (update.Message!.Text?.StartsWith("/") ?? false)
                    {
                        await HandleCommand(botClient, update, cancellationToken);
                        return true;
                    }
                    else if (update.Message!.Chat.Type == ChatType.Private && !string.IsNullOrEmpty(update.Message!.Text))
                    {
                        await HandleAction(botClient, update, cancellationToken);
                        return true;
                    }

                    return false;
                default:
                    return false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Exception during handling telegram update");
            return false;
        }
    }

    private async Task HandleState(ITelegramBotClient botClient, Update update, string state,
        CancellationToken cancellationToken)
    {
        var args = update.Message.Text;
        var context = new ModuleContext(botClient, this, update, args, state);

        if (_states.TryGetValue(state, out var stateInfo))
        {
            _logger.LogDebug("Executing state {state} from {module}", state, stateInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = stateInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                module.Context = context;

                // Method
                Result result;
                try
                {
                    await InvokeStateHandler(stateInfo, module, scope.ServiceProvider, args);
                    result = Result.FromSuccess();
                }
                catch (BaseCommandException e)
                {
                    result = Result.FromError(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during executing state {state} from {module}", state,
                        stateInfo.Module.Type.Name);
                    result = Result.FromError(e);
                }

                if (StateExecuted != null) await StateExecuted.InvokeAsync(stateInfo, context, result);
            }
        }

        else
        {
            if (StateExecuted != null)
                await StateExecuted.InvokeAsync(null, context, Result.FromError(new UnknownCommand()));
            await ChangeStateAsync(update.Message.Chat.Id, "/");
        }
    }
    
    private async Task InvokeStateHandler(StateInfo stateInfo, BaseTelegramModule module, IServiceProvider scoped, string? args)
    {
        var returnTask = stateInfo.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task));

        var parametersInfos = stateInfo.MethodInfo.GetParameters();
        var parameters = new object[parametersInfos.Length];

        if (stateInfo.ParseArgs)
        {
            var splittedArgs = _splitter.Split(args);
            parameters = await ParseParameters(parametersInfos, splittedArgs, module.Context);
        }
        else
        {
            if (parametersInfos.Length > 1 || parametersInfos.Length < 1 || parametersInfos[0].ParameterType != typeof(string)) 
                throw new Exception("State handler method contains wrong arguments");

            parameters[0] = args;
        }

        var result = stateInfo.MethodInfo.Invoke(module, parameters);

        if (returnTask && stateInfo.RunMode == RunMode.Sync)
            await (result as Task);
    }


    private async Task HandleCommand(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var args = update.Message.Text!.Split(' ');
        var commandString = args[0].Replace($"@{_host.User.Username}", "");
        var argsString = string.Join(' ', args.Skip(1).ToArray());
        var context = new ModuleContext(botClient, this, update, argsString, commandString);

        if (_commands.TryGetValue(commandString, out var commandInfo))
        {
            _logger.LogDebug("Executing command {command} from {module}", commandInfo.Name,
                commandInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = commandInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                module.Context = context;

                // Method
                Result result;
                try
                {
                    await InvokeCommand(commandInfo, module, scope.ServiceProvider, argsString);
                    result = Result.FromSuccess();
                }
                catch (BaseCommandException e)
                {
                    result = Result.FromError(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during executing command {command} from {module}", commandInfo.Name,
                        commandInfo.Module.Type.Name);
                    result = Result.FromError(e);
                }

                if (CommandExecuted != null) await CommandExecuted.InvokeAsync(commandInfo, context, result);
            }
        }
        else
        {
            if (CommandExecuted != null)
                await CommandExecuted.InvokeAsync(null, context, Result.FromError(new UnknownCommand()));
        }
    }

    private async Task InvokeCommand(CommandInfo commandInfo, BaseTelegramModule module, IServiceProvider scoped,
        string args)
    {
        var returnTask = commandInfo.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task));

        var parametersInfos = commandInfo.MethodInfo.GetParameters();
        var splittedArgs = _splitter.Split(args);
        
        var parameters = await ParseParameters(parametersInfos, splittedArgs, module.Context);

        var result = commandInfo.MethodInfo.Invoke(module, parameters);

        if (returnTask && commandInfo.RunMode == RunMode.Sync)
            await (result as Task);
    }

    private async Task HandleAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var actionString = update.Message.Text;
        var context = new ModuleContext(botClient, this, update, "", actionString);
        if (_actions.TryGetValue(actionString, out var actionInfo))
        {
            _logger.LogDebug("Executing action {action} from {module}", actionInfo.Name, actionInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = actionInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                module.Context = context;

                // Method
                Result result;
                try
                {
                    await InvokeAction(actionInfo, module, scope.ServiceProvider);
                    result = Result.FromSuccess();
                }
                catch (BaseCommandException e)
                {
                    result = Result.FromError(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during executing cation {action} from {module}", actionInfo.Name,
                        actionInfo.Module.Type.Name);
                    result = Result.FromError(e);
                }

                if (ActionExecuted != null) await ActionExecuted.InvokeAsync(actionInfo, context, result);
            }
        }
        else
        {
            if (ActionExecuted != null)
                await ActionExecuted.InvokeAsync(null, context, Result.FromError(new UnknownCommand()));
        }
    }

    private async Task<object[]> ParseParameters(ParameterInfo[] parametersInfos, List<string> splittedArgs, ModuleContext context)
    {
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
                throw new BadArgs(i, parametersInfos.Length, splittedArgs.Count());
            }

            var reader = _provider
                .GetServices<ITypeReader>()
                .First(r => r.Type == type);
            var read = await reader.ReadTypeAsync(context, splittedArgs[i]);
            if (!read.Success)
            {
                throw new TypeConvertException(read.ErrorReason, parametersInfos[i], i);
            }

            parameters[i] = read.Result;
        }

        return parameters;
    }

    private async Task InvokeAction(ActionInfo actionInfo, BaseTelegramModule module, IServiceProvider scoped)
    {
        var returnTask = actionInfo.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task));

        var parametersInfos = actionInfo.MethodInfo.GetParameters();
        var parameters = new object[parametersInfos.Length];

        if (parametersInfos.Length > 0)
        {
            throw new ArgumentException("Action cannot take parameters");
        }

        var result = actionInfo.MethodInfo.Invoke(module, parameters);

        if (returnTask && actionInfo.RunMode == RunMode.Sync)
            await (result as Task);
    }
    
    
    /// <summary>
    /// Adds single module
    /// </summary>
    /// <typeparam name="T">Class inherited from <see cref="T:TelegramModularFramework.Modules.BaseTelegramModule"/></typeparam>
    public void AddModule<T>() where T : BaseTelegramModule
    {
        AddModuleInternal(typeof(T));
    }

    
    /// <summary>
    /// Adds single module
    /// </summary>
    /// <param name="module">Type of class inherited from <see cref="T:TelegramModularFramework.Modules.BaseTelegramModule"/></param>
    /// <exception cref="Exception">Class has wrong definition</exception>
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
        _modules.Add(moduleInfo);
        _logger.LogDebug("Module {module} added", module);

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

        foreach (var command in commands)
        {
            if (!_commands.TryAdd("/" + command.Name, command))
            {
                throw new Exception($"Command with name {command.Name} already exists");
            }

            _logger.LogDebug("Command {command} added", command.Name);
        }

        var actions = module
            .GetMethods()
            .Where(m => m.IsDefined(typeof(ActionAttribute)))
            .Select(m => new ActionInfo()
            {
                MethodInfo = m,
                Attributes = m.GetCustomAttribute<ActionAttribute>(),
                Module = moduleInfo,
                Name = m.GetCustomAttribute<ActionAttribute>().Name ??
                       Regex.Replace(m.Name, "([A-Z])", " $1", RegexOptions.Compiled).Trim(),
                Summary = m.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "",
                RunMode = m.GetCustomAttribute<RunModeAttribute>()?.RunMode ?? RunMode.Sync
            });

        foreach (var action in actions)
        {
            if (!_actions.TryAdd(action.Name, action))
            {
                throw new Exception($"Action with name {action.Name} already exists");
            }

            _logger.LogDebug("Action {action} added", action.Name);
        }

        if (module.IsDefined(typeof(StateAttribute)))
        {
            var methods = module.GetMethods().Where(m => m.IsDefined(typeof(StateHandlerAttribute)));
            if (methods.Count() == 0) throw new Exception($"No state handler defined in {module}");
            if (methods.Count() > 1) throw new Exception($"Multiple state handlers defined in {module}");

            var stateName = module.GetCustomAttribute<StateAttribute>().Name;
            Type type = module;
            while (type.IsNested)
            {
                type = type.DeclaringType;
                if (type.IsDefined(typeof(StateAttribute)))
                {
                    stateName = type.GetCustomAttribute<StateAttribute>().Name + "/" + stateName;
                }
            }

            var stateInfo = new StateInfo()
            {
                Module = moduleInfo,
                MethodInfo = methods.First(),
                Name = stateName,
                Summary = methods.First().GetCustomAttribute<SummaryAttribute>()?.Summary ?? "",
                RunMode = methods.First().GetCustomAttribute<RunModeAttribute>()?.RunMode ?? RunMode.Sync,
                ParseArgs = methods.First().GetCustomAttribute<StateHandlerAttribute>().ParseArgs,
            };

            if (!_states.TryAdd("/" + stateName, stateInfo))
            {
                throw new Exception($"State with name {stateName} already exists");
            }

            _logger.LogDebug("State {state} added", stateName);
        }

        foreach (var nestedType in module
                     .GetNestedTypes()
                     .Where(t => t.IsNestedPublic && !t.IsAbstract && t.IsSubclassOf(typeof(BaseTelegramModule)))
                 )
        {
            AddModuleInternal(nestedType);
        }
    }

    
    /// <summary>
    /// Adds all modules from given assembly.
    /// If assembly is null, the calling assembly is taken.
    /// </summary>
    /// <param name="assembly">Assembly to get modules.</param>
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

    /// <summary>
    /// Sets list of commands via telegram bot api
    /// See <see cref="Telegram.Bot.TelegramBotClientExtensions.SetMyCommandsAsync"/> for details
    /// </summary>
    public async Task SetMyCommands()
    {
        var commands = _commands
            .Where(c => !c.Value.HiddenFromList && !string.IsNullOrEmpty(c.Value.Summary))
            .Select(c => new BotCommand()
            {
                Command = c.Value.Name,
                Description = c.Value.Summary
            });
        _logger.LogDebug("Adding commands list ({commands})", commands.Select(c => c.Command));
        await _client.SetMyCommandsAsync(commands);
    }

    
    /// <summary>
    /// Changes state of user by chatId and path
    /// </summary>
    /// <param name="chatId">Chat Id</param>
    /// <param name="path">Relative or absolute path</param>
    public async Task ChangeStateAsync(long chatId, string path)
    {
        string getPath = "";
        if (path.StartsWith("/"))
        {
            var baseUri = new Uri("state://");
            getPath = new Uri(baseUri, path).AbsolutePath;
        }
        else
        {
            var currentPath = await _stateHolder.GetState(chatId);
            var baseUri = new Uri("state://" + currentPath + (currentPath?.EndsWith("/") ?? false ? "" : "/"));
            getPath = new Uri(baseUri, path).AbsolutePath;
        }

        _stateHolder.SetState(chatId, getPath);
    }
}