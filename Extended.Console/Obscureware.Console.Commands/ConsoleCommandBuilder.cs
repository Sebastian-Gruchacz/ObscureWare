namespace Obscureware.Console.Commands
{
    using System;
    using System.Linq;

    internal class ConsoleCommandBuilder
    {
        /// <summary>
        /// Validates command definitions and creates instance
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns>Tuple: commandName, CommandModel, Command instance</returns>
        public Tuple<string, ModelBuilder, IConsoleCommand> ValidateAndBuildCommand(Type commandType)
        {
            // 1. Verify command type
            if (commandType == null)
                throw new ArgumentNullException(nameof(commandType));
            if (commandType.IsAbstract || commandType.IsInterface)
                throw new ArgumentException($"Command type cannot be abstract or interface.", nameof(commandType));
            if (!typeof(IConsoleCommand).IsAssignableFrom(commandType))
                throw new ArgumentException($"Command type must implement {nameof(IConsoleCommand)} interface.",
                    nameof(commandType));

            // 1a. Model attribute check
            var modelAtt =
                (CommandModelAttribute)
                commandType.GetCustomAttributes(typeof(CommandModelAttribute), inherit: true).FirstOrDefault();
            if (modelAtt == null)
                throw new ArgumentException($"Command type must be decorated with {nameof(CommandModelAttribute)}.",
                    nameof(commandType));

            // 2. Verify Command's model type
            Type modelType = modelAtt.ModelType;
            if (modelType == null)
                throw new ArgumentException($"Model type of {nameof(CommandModelAttribute)} cannot be null.",
                    nameof(commandType));
            if (modelType.IsAbstract || modelType.IsInterface)
                throw new ArgumentException($"Model type cannot be abstract or interface.", nameof(commandType));
            if (!typeof(CommandModel).IsAssignableFrom(modelType))
                throw new ArgumentException($"Model type must inherit from {nameof(CommandModel)}.", nameof(commandType));

            // 2a. Model reverse pointer attribute check
            var reverseTypeAtt =
                (CommandDescriptorForAttribute)
                modelType.GetCustomAttributes(typeof(CommandDescriptorForAttribute), inherit: true).FirstOrDefault();
            if (reverseTypeAtt == null)
                throw new ArgumentException(
                    $"Model type must be decorated with {nameof(CommandDescriptorForAttribute)}.", nameof(commandType));
            if (reverseTypeAtt.ModelledCommandType != commandType)
                throw new ArgumentException(
                    $"Model type attribute {nameof(CommandDescriptorForAttribute)} must point back to the command type.",
                    nameof(commandType));

            // 2b. Model, command name attribute check
            var modelNameAtt =
                (CommandNameAttribute)
                modelType.GetCustomAttributes(typeof(CommandNameAttribute), inherit: true).FirstOrDefault();
            if (modelNameAtt == null)
                throw new ArgumentException($"Model type must be decorated with {nameof(CommandNameAttribute)}.",
                    nameof(commandType));
            if (string.IsNullOrWhiteSpace(modelNameAtt.CommandName))
                throw new ArgumentException($"Model type must be decorated with {nameof(CommandNameAttribute)}.",
                    nameof(commandType));
            string commandName = modelNameAtt.CommandName;

            // 3. Validate model type definition details
            var mBuilder = this.ValidateModel(modelType);

            // 4. Try creating instance of the command.
            IConsoleCommand instance;
            try
            {
                instance = (IConsoleCommand)Activator.CreateInstance(commandType);
            }
            catch (Exception iex)
            {
                throw new InvalidOperationException($"Could not create instance of the command {commandType.FullName} using default activator. Make sure it exposes public, parameterless constructor.", iex);
            }

            return new Tuple<string, ModelBuilder, IConsoleCommand>(commandName, mBuilder, instance);
        }

        private ModelBuilder ValidateModel(Type modelType)
        {
            return new ModelBuilder(modelType);
        }
    }
}