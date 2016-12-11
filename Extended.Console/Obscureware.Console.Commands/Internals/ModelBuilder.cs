namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    internal class ModelBuilder
    {
        public string CommandName { get; private set; }

        private readonly Type _modelType;

        public ModelBuilder(Type modelType, string commandName)
        {
            CommandName = commandName;
            if (modelType == null) throw new ArgumentNullException(nameof(modelType));

            _modelType = modelType;
            this.ValidateModel(modelType);

            // TODO: cache help data
            // TODO: cache parsing data
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

        public CommandModel BuildModel(IEnumerable<string> arguments)
        {
            // TODO: full implementation

            return Activator.CreateInstance(_modelType) as CommandModel;
        }


        public void PrintHelp(ICommandOutput output, CommandEngineStyles styles, IEnumerable<string> arguments)
        {
            // TODO: implement
        }
    }
}