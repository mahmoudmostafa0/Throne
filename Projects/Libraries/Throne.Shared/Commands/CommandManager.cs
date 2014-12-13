using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Throne.Shared.Exceptions;
using Throne.Shared.Reflection;
using Throne.Shared.Security.Permissions;
using Throne.Shared.Threading;

namespace Throne.Shared.Commands
{
    public sealed class CommandManager : SingletonActor<CommandManager>
    {
        private readonly object _cmdLock = new object();

        private readonly Dictionary<string, Command> _commands =
            new Dictionary<string, Command>(StringComparer.OrdinalIgnoreCase);

        private CommandManager()
        {
            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                LoadCommands(asm);
        }

        public IDictionary<string, Command> Commands
        {
            get { return new Dictionary<string, Command>(_commands); }
        }

        public void AddCommand(Command cmd, params string[] triggers)
        {
            foreach (string trigger in triggers)
                _commands.Add(trigger, cmd);
        }

        public void RemoveCommand(params string[] triggers)
        {
            foreach (string trigger in triggers)
                _commands.Remove(trigger);
        }

        public Command GetCommand(string trigger)
        {
            return _commands.TryGet(trigger);
        }

        public void ExecuteCommand(CommandArguments arguments, ICommandUser sender)
        {
            String cmd = "";

            try
            {
                cmd = arguments.NextString();

                if (string.IsNullOrWhiteSpace(cmd))
                    return;

                Command command = GetCommand(cmd);
                if (command == null)
                {
                    sender.Respond("Unknown command: {0}".Interpolate(cmd));
                    return;
                }

                if (command.RequiresSender && sender == null)
                {
                    ExceptionManager.RegisterException(new Exception("sender cannot be null for this command"));
                    return;
                }

                var permissions = command.RequestedPermissions;
                if (sender != null && permissions != null && !permissions.Contains(typeof(ConsolePermission)) &&
                    !permissions.Any(sender.HasPermission))
                {
                    sender.Respond("Command {0} requires permission {1}.".Interpolate(cmd, permissions));
                    return;
                }

                lock (_cmdLock)
                    command.Execute(arguments, sender);
            }
            catch (CommandArgumentException ex)
            {
                sender.Respond("Argument error for command \"{0}\" : {1}".Interpolate(cmd, ex.Message));
            }
            catch (Exception ex)
            {
                ExceptionManager.RegisterException(ex);
            }
        }


        public void LoadCommands(Assembly asm)
        {
            Type cmdType = typeof(Command);

            foreach (Type type in asm.GetTypes())
            {
                var attr = type.GetCustomAttribute<CommandAttribute>();
                if (attr == null)
                    continue;

                if (!type.IsAssignableTo(cmdType))
                    throw new ReflectionException("A command class must inherit {0}.".Interpolate(cmdType));

                if (type.IsGenericType)
                    throw new ReflectionException("A command class cannot be generic.");

                if (type.IsAbstract)
                    throw new ReflectionException("A command class cannot be abstract.");

                ConstructorInfo ctor = type.GetConstructors().FirstOrDefault(x => x.GetParameters().Length == 0);
                if (ctor == null)
                    throw new ReflectionException("A command class must have a public parameterless constructor.");

                var cmd = (Command)ctor.Invoke(null);
                AddCommand(cmd, attr.Triggers);
            }
        }
    }
}