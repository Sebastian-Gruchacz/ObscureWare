namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal class CommandManager
    {
        private readonly Dictionary<string, CommandInfo> _commands;

        public CommandCaseSensitivenes CommandsSensitivenes { get; set; }

        public CommandManager(Type[] commands)
        {
            _commands = CheckCommands(commands);
        }

        public CommandInfo FindCommand(string cmdName)
        {
            return this._commands
                .SingleOrDefault(pair =>
                    pair.Key.Equals(cmdName,
                        (CommandsSensitivenes == CommandCaseSensitivenes.Sensitive)
                            ? StringComparison.InvariantCulture
                            : StringComparison.InvariantCultureIgnoreCase))
                .Value;
        }

        public IEnumerable<CommandInfo> GetAll()
        {
            return this._commands.Values.ToArray();
        }

        private Dictionary<string, CommandInfo> CheckCommands(Type[] commands)
        {
            Dictionary<string, CommandInfo> result = new Dictionary<string, CommandInfo>();

            ConsoleCommandBuilder builder = new ConsoleCommandBuilder();
            foreach (var commandType in commands)
            {
                Tuple<ModelBuilder, IConsoleCommand> cmd = builder.ValidateAndBuildCommand(commandType);
                result.Add(cmd.Item1.CommandName.ToUpper(), new CommandInfo(cmd.Item2, cmd.Item1));
            }

            return result;
        }


    }
}