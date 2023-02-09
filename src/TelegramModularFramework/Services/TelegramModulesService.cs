using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
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
using TelegramModularFramework.Services.Globalization;
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
    private readonly TelegramBotUser _telegramBotUser;
    private readonly IStringSplitter _splitter;
    private readonly ITelegramBotClient _client;
    private readonly TelegramModulesConfiguration _config;
    private readonly IStateHolder _stateHolder;
    private readonly ICultureInfoUpdater _cultureInfoUpdater;

    private List<ModuleInfo> _modules = new();

    /// <summary>
    /// List of all loaded modules
    /// </summary>
    public List<ModuleInfo> Modules => _modules.ToList();

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

    private List<CallbackQueryHandlerInfo> _callbackQueryHandlers = new();

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
    
    /// <summary>
    /// Async event that executes after state executed successfully or not
    /// </summary>
    public event Func<CallbackQueryHandlerInfo?, ModuleContext, Result, Task> CallbackExecuted;

    public TelegramModulesService(IServiceProvider provider, ILogger<TelegramModulesService> logger,
        TelegramBotUser telegramBotUser, IStringSplitter splitter, ITelegramBotClient client,
        IOptions<TelegramModulesConfiguration> config, IStateHolder stateHolder, ICultureInfoUpdater cultureInfoUpdater)
    {
        _provider = provider;
        _logger = logger;
        _telegramBotUser = telegramBotUser;
        _splitter = splitter;
        _client = client;
        _config = config.Value;
        _stateHolder = stateHolder;
        _cultureInfoUpdater = cultureInfoUpdater;
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
                case UpdateType.CallbackQuery:
                    await HandleCallbackQuery(botClient, update, cancellationToken);
                    return true;
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
        var context = new ModuleContext(botClient, this, update, args, state, null);
        CultureInfo.CurrentCulture = _cultureInfoUpdater.GetCultureInfo(context);

        if (_states.TryGetValue(state, out var stateInfo))
        {
            _logger.LogDebug("Executing state {state} from {module}", state, stateInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = stateInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                context.Group = stateInfo.Module.Group;
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
        var commandString = args[0].Replace($"@{_telegramBotUser.User?.Username}", "");
        var argsString = string.Join(" ", args.Skip(1).ToList());
        var context = new ModuleContext(botClient, this, update, argsString, commandString, null);
        CultureInfo.CurrentCulture = _cultureInfoUpdater.GetCultureInfo(context);

        if (_commands.TryGetValue(commandString, out var commandInfo))
        {
            _logger.LogDebug("Executing command {command} from {module}", commandInfo.Name,
                commandInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = commandInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                context.Group = commandInfo.Module.Group;
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
        var context = new ModuleContext(botClient, this, update, "", actionString, null);
        CultureInfo.CurrentCulture = _cultureInfoUpdater.GetCultureInfo(context);
        
        if (_actions.TryGetValue(actionString, out var actionInfo))
        {
            _logger.LogDebug("Executing action {action} from {module}", actionInfo.Name, actionInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = actionInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                context.Group = actionInfo.Module.Group;
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
                    _logger.LogError(e, "Exception during executing action {action} from {module}", actionInfo.Name,
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
    
    private async Task HandleCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        var context = new ModuleContext(botClient, this, update, null, update.CallbackQuery.Data, null);
        CultureInfo.CurrentCulture = _cultureInfoUpdater.GetCultureInfo(context);

        var callbackQueryHandlerInfo = _callbackQueryHandlers
            .Select(c => new
            {
                c = c,
                pattern = PatternFromPath(c.Name)
            })
            .Where(c => Regex.IsMatch(update.CallbackQuery.Data, c.pattern))
            .Select(c => c.c)
            .FirstOrDefault();
        
        if (callbackQueryHandlerInfo != null)
        {
            _logger.LogDebug("Executing callback query handler {handler} from {module}", callbackQueryHandlerInfo.Name, callbackQueryHandlerInfo.Module.Type.Name);
            using (var scope = _provider.CreateScope())
            {
                // Context
                var module = callbackQueryHandlerInfo.Module.Factory.Invoke(scope.ServiceProvider, null) as BaseTelegramModule;
                context.Group = callbackQueryHandlerInfo.Module.Group;
                module.Context = context;

                // Method
                Result result;
                try
                {
                    await InvokeCallbackHandler(callbackQueryHandlerInfo, module, update.CallbackQuery.Data, scope.ServiceProvider);
                    result = Result.FromSuccess();
                }
                catch (BaseCommandException e)
                {
                    result = Result.FromError(e);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Exception during executing cation {action} from {module}", callbackQueryHandlerInfo.Name,
                        callbackQueryHandlerInfo.Module.Type.Name);
                    result = Result.FromError(e);
                }

                if (CallbackExecuted != null) await CallbackExecuted.InvokeAsync(callbackQueryHandlerInfo, context, result);
            }
        }
        else
        {
            if (CallbackExecuted != null)
                await CallbackExecuted.InvokeAsync(null, context, Result.FromError(new UnknownCommand()));
            await ChangeStateAsync(update.Message.Chat.Id, "/");
        }
    }

    private string PatternFromPath(string path)
    {
        // /sample/{data:*} => /sample/*

        return "/" + string.Join("/", ParsePath(path).Select(p => p.Template));
    }

    private string InsertParametersIntoPath(string path, object? parameters)
    {
        if (parameters == null) return path;
        
        var parts = ParsePath(path);
        var dictionary = parameters.ToDictionary();
        var usedParams = new List<string>();
        var result = new StringBuilder();
        foreach (var part in parts)
        {
            result.Append("/");
            if (part.Dynamic)
            {
                if (!dictionary.TryGetValue(part.Name, out var value))
                    throw new ArgumentException($"{part.Name} was not present", nameof(parameters));

                usedParams.Add(part.Name);
                result.Append(value);
            }
            else
            {
                result.Append(part.Name);
            }
        }

        var queryParams = dictionary.Where(e => !usedParams.Contains(e.Key)).ToDictionary(x => x.Key, x => x.Value);
        result.Append(queryParams.ToQueryString());
        
        return result.ToString();
    }

    private IEnumerable<PathPart> ParsePath(string path)
    {
        return path
            .Split('/')
            .Skip(1) // Starts from '/'
            .Select(p => new
            {
                Match = Regex.Match(p, @"\{([^()]+)\}"),
                Path = p
            })
            .Select(p => new
            {
                Match = p.Match,
                Path = p.Path,
                Split = p.Match.Success ? p.Match.Value.Remove(0, 1).Remove(p.Match.Value.Length - 2, 1).Split(':') : null
            })
            .Select(p => new PathPart()
            {
                Dynamic = p.Match.Success,
                Name = p.Split?[0] ?? p.Path,
                Template = p.Match.Success ? (p.Split?.Length > 0 ? string.Join(":", p.Split.Skip(1)) : "*") : p.Path
            });
        
        // return Regex
        //     .Matches(path, @"\{([^()]+)\}")
        //     .Select(m => m.Value.Split(':'))
        //     .Select(m => new PathPart()
        //     {
        //         Name = m[0],
        //         Template = m.Length > 0 ? string.Join(':', m.Skip(1)) : "*"
        //     });
    }

    public async Task InvokeCallbackHandler(CallbackQueryHandlerInfo callbackQueryHandlerInfo, BaseTelegramModule module, string data, IServiceProvider scoped)
    {
        var returnTask = callbackQueryHandlerInfo.MethodInfo.ReturnType.IsAssignableFrom(typeof(Task));

        var parametersInfos = callbackQueryHandlerInfo.MethodInfo.GetParameters();
        var parameters = new object[parametersInfos.Length];

        var dataSplit = data.Split('?')[0].Split('/').Skip(1).ToArray();
        var pathParts = ParsePath(callbackQueryHandlerInfo.Name).ToArray();
        var query = data.Split('?').Length > 1 ? HttpUtility.ParseQueryString("?" + data.Split('?')[1]) : null;

        foreach (var parameterInfo in parametersInfos)
        {
            string value;
            var i = Array.FindIndex(pathParts, p => p.Dynamic && p.Name.ToLower() == parameterInfo.Name.ToLower());
            if (i == -1)
            {
                value = query?[parameterInfo.Name.ToLower()];
                if (value == null)
                {
                    throw new CallbackQueryHandlerBadPath(pathParts, parameterInfo, data);
                }
            }
            else
            {
                value = dataSplit[i];
            }
            
            var reader = _provider
                .GetServices<ITypeReader>()
                .First(r => r.Type == parameterInfo.ParameterType);
            var read = await reader.ReadTypeAsync(module.Context, value);
            if (!read.Success)
            {
                throw new TypeConvertException(read.ErrorReason, parameterInfo, i);
            }

            parameters[parameterInfo.Position] = read.Result;
        }

        var result = callbackQueryHandlerInfo.MethodInfo.Invoke(module, parameters);

        if (returnTask && callbackQueryHandlerInfo.RunMode == RunMode.Sync)
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
        var groupName = "/" +  module.GetCustomAttribute<GroupAttribute>()?.Name ?? "";
        Type type = module;
        while (type.IsNested)
        {
            type = type.DeclaringType;
            if (type.IsDefined(typeof(GroupAttribute)))
            {
                groupName = "/" + type.GetCustomAttribute<GroupAttribute>().Name + groupName;
            }
        }
        
        var moduleInfo = new ModuleInfo()
        {
            Type = module,
            Factory = ActivatorUtilities.CreateFactory(module, new Type[] { }),
            Group = groupName
        };
        _modules.Add(moduleInfo);
        _logger.LogDebug("Module {module} added", module);

        var commands = module
            .GetMethods()
            .Where(m => m.IsDefined(typeof(CommandAttribute)))
            ?.Select(m => new CommandInfo()
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
            if (_commands.ContainsKey("/" + command.Name))
            {
                throw new Exception($"Command with name {command.Name} already exists");
            }
            else
            {
                _commands.Add("/" + command.Name, command);
            }

            _logger.LogDebug("Command {command} added", command.Name);
        }

        var actions = module
            .GetMethods()
            .Where(m => m.IsDefined(typeof(ActionAttribute)))
            ?.Select(m => new ActionInfo()
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
            if (_actions.ContainsKey(action.Name))
            {
                throw new Exception($"Action with name {action.Name} already exists");
            }
            else
            {
                _actions.Add(action.Name, action);
            }

            _logger.LogDebug("Action {action} added", action.Name);
        }
        
        var stateHandlers = module.GetMethods().Where(m => m.IsDefined(typeof(StateHandlerAttribute)));
        if (stateHandlers.Count() == 1 && groupName == "/") throw new Exception($"State handler must be in Group");
        if (stateHandlers.Count() > 1) throw new Exception($"Multiple state handlers defined in {module} ({groupName})");
        if (stateHandlers.Count() == 1)
        {
            var stateInfo = new StateInfo()
            {
                Module = moduleInfo,
                MethodInfo = stateHandlers.First(),
                Name = groupName,
                Summary = stateHandlers.First().GetCustomAttribute<SummaryAttribute>()?.Summary ?? "",
                RunMode = stateHandlers.First().GetCustomAttribute<RunModeAttribute>()?.RunMode ?? RunMode.Sync,
                ParseArgs = stateHandlers.First().GetCustomAttribute<StateHandlerAttribute>().ParseArgs,
            };

            if (_states.ContainsKey(groupName))
            {
                throw new Exception($"State handler in group {groupName} already exists");
            }
            else
            {
                _states.Add(groupName, stateInfo);
            }

            _logger.LogDebug("State handler in group {state} added", groupName);
        }

        var callbackQueryHandlers = module
            .GetMethods()
            .Where(m => m.IsDefined(typeof(CallbackQueryHandlerAttribute)))
            ?.Select(m => new CallbackQueryHandlerInfo()
            {
                Module = moduleInfo,
                MethodInfo = m,
                Name = groupName + "/" + m.GetCustomAttribute<CallbackQueryHandlerAttribute>().Path,
                Summary = m.GetCustomAttribute<SummaryAttribute>()?.Summary ?? "",
                RunMode = m.GetCustomAttribute<RunModeAttribute>()?.RunMode ?? RunMode.Sync,
                Attributes = m.GetCustomAttribute<CallbackQueryHandlerAttribute>(),
            });

        foreach (var callbackQueryHandlerInfo in callbackQueryHandlers)
        {
            if (_callbackQueryHandlers.Count(c => c.Name == callbackQueryHandlerInfo.Name) > 0)
            {
                throw new Exception($"Callback query handler with path {callbackQueryHandlerInfo.Name} already exists");
            }

            _callbackQueryHandlers.Add(callbackQueryHandlerInfo);
            _logger.LogDebug("Callback query handler {path} added", callbackQueryHandlerInfo.Name);
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

        await _stateHolder.SetState(chatId, getPath);
    }
    
    public string UrlFor(Type type, object? parameters = null)
    {
        var module = _modules.FirstOrDefault(m => m.Type == type);
        if (module == null) throw new ArgumentException("Module not found", nameof(type));
        return InsertParametersIntoPath(module.Group, parameters);
    }
    
    public string UrlFor<TModule>(object? parameters = null) where TModule : BaseTelegramModule => UrlFor(typeof(TModule), parameters);

    public string UrlFor(Type type, string handler, object? parameters = null)
    {
        var module = _modules.FirstOrDefault(m => m.Type == type);
        if (module == null) throw new ArgumentException("Module not found", nameof(type));

        var handlerInfo = _callbackQueryHandlers.FirstOrDefault(e => e.Module == module && e.MethodInfo.Name == handler);
        if (handlerInfo == null)
        {
            if (_states.Count(s => s.Value.MethodInfo.Name == handler) != 0)
            {
                return InsertParametersIntoPath(module.Group, parameters);
            }

            throw new ArgumentException("Callback handler or state handler not found", nameof(handler));
        }

        return InsertParametersIntoPath(module.Group + "/" + handlerInfo.Attributes.Path, parameters);
    }
    
    public string UrlFor<TModule>(string handler, object? parameters = null) where TModule : BaseTelegramModule => UrlFor(typeof(TModule), handler, parameters);
}