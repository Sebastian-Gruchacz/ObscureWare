namespace Obscureware.Console.Commands
{
    using System;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>https://en.wikipedia.org/wiki/Command-line_interface</remarks>
    public interface ICommandParserOptions
    {
        string[] FlagCharacters { get; }

        string[] SwitchCharacters { get; }

        CommandOptionArgumentMode OptionArgumentMode { get; }

        char OptionArgumentJoinCharacater { get; }

        bool AllowFlagsAsOneArgument { get; }

        UnnamedOptionsMode UnnamedOptionsMode { get; }

        CommandCaseSensitivenes CommandsSensitivenes { get; }

        void ValidateParserOptions();
    }

    public class CommandParserOptions : ICommandParserOptions
    {
        public string[] FlagCharacters { get; set; } = new[] { @"\" };

        public string[] SwitchCharacters { get; set; } = new[] { @"-" };

        public CommandOptionArgumentMode OptionArgumentMode { get; set; } = CommandOptionArgumentMode.Separated;

        public char OptionArgumentJoinCharacater { get; set; } = ':';

        public bool AllowFlagsAsOneArgument { get; set; } = false;

        public UnnamedOptionsMode UnnamedOptionsMode { get; set; } = UnnamedOptionsMode.EndOnly;

        public CommandCaseSensitivenes CommandsSensitivenes { get; set; } = CommandCaseSensitivenes.Insensitive;

        public void ValidateParserOptions()
        {
            if (this.FlagCharacters == null || this.FlagCharacters.Length < 1)
            {
                throw new BadImplementationException($"{nameof(this.FlagCharacters)} cannot be null or empty.", typeof(CommandParserOptions));
            }

            if (this.SwitchCharacters == null || this.SwitchCharacters.Length < 1)
            {
                throw new BadImplementationException($"{nameof(this.SwitchCharacters)} cannot be null or empty.", typeof(CommandParserOptions));
            }

            if (this.FlagCharacters.Any(string.IsNullOrWhiteSpace))
            {
                throw new BadImplementationException($"{nameof(this.FlagCharacters)} cannot contain null or empty elements.", typeof(CommandParserOptions));
            }

            if (this.SwitchCharacters.Any(string.IsNullOrWhiteSpace))
            {
                throw new BadImplementationException($"{nameof(this.SwitchCharacters)} cannot contain null or empty elements.", typeof(CommandParserOptions));
            }

            if ((this.OptionArgumentMode == CommandOptionArgumentMode.Joined) && char.IsWhiteSpace(this.OptionArgumentJoinCharacater))
            {
                throw new BadImplementationException($"In {nameof(CommandOptionArgumentMode.Joined)} mode {nameof(this.OptionArgumentJoinCharacater)} cannot be white character.", typeof(CommandParserOptions));
            }

            if (this.AllowFlagsAsOneArgument && this.OptionArgumentMode == CommandOptionArgumentMode.Merged)
            {
                throw new BadImplementationException($"When {nameof(this.OptionArgumentMode)} is set to \"{nameof(CommandOptionArgumentMode.Merged)}\", {nameof(this.AllowFlagsAsOneArgument)} cannot be set to TRUE because this could led to ambiguous syntax.", typeof(CommandParserOptions));
            }

            if (this.UnnamedOptionsMode == UnnamedOptionsMode.Mixed && this.OptionArgumentMode == CommandOptionArgumentMode.Separated)
            {
                throw new BadImplementationException($"Options {nameof(this.OptionArgumentMode)}=\"{nameof(CommandOptionArgumentMode.Separated)}\" and {nameof(this.UnnamedOptionsMode)}=\"{nameof(UnnamedOptionsMode.Mixed)}\" should not be selected at the same time because this could led to ambiguous syntax.", typeof(CommandParserOptions));
            }
        }

        // TODO: default static constructors for DOS-style, Unix-style etc...
    }
}