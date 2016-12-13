namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Model;

    internal class ModelBuilder
    {
        public string CommandName { get; private set; }
        public string CommandDescription { get; private set; }

        private readonly Type _modelType;

        public ModelBuilder(Type modelType, string commandName)
        {
            this.CommandName = commandName;
            if (modelType == null) throw new ArgumentNullException(nameof(modelType));

            this._modelType = modelType;
            this.ValidateModel(modelType);

            this.ReadHelpInformation();
            // TODO: cache parsing data
        }

        private void ReadHelpInformation()
        {
            CommandDescriptionAttribute att = this._modelType.GetCustomAttribute<CommandDescriptionAttribute>();
            this.CommandDescription = att?.Description ?? "* Description not available *";

            // TODO: more elements to be read - switches, arguments, enumerations, description, syntaxes, etc - to be stored in dedicated HelpContainer for future use
        }

        private void ValidateModel(Type modelType)
        {
            var publicCtor = modelType.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, CallingConventions.Any, new Type[0], null);
            if (publicCtor == null)
            {
                throw new ArgumentException($"{modelType.FullName} does not expose public, parameterless constructor.");
            }



            // TODO: validate model type
        }

        public CommandModel BuildModel(IEnumerable<string> arguments, ICommandParserOptions options)
        {
            var model = Activator.CreateInstance(this._modelType) as CommandModel;
            string[] args = arguments.ToArray();
            int argIndex = 0;
            while (argIndex < args.Length)
            {



                argIndex++;
            }


            // TODO: full implementation

            return model;
        }


        public void PrintHelp(ICommandOutput output, CommandEngineStyles styles, IEnumerable<string> arguments)
        {
            // TODO: implement
        }
    }
}