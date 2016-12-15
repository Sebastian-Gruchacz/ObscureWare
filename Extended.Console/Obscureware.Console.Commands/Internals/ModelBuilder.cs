﻿namespace Obscureware.Console.Commands.Internals
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Converters;
    using Model;
    using Parsers;

    internal class ModelBuilder
    {
        private readonly Dictionary<string, FlagPropertyParser> _flagParsers = new Dictionary<string, FlagPropertyParser>();
        private readonly Dictionary<string, BaseSwitchPropertyParser> _switchParsers = new Dictionary<string, BaseSwitchPropertyParser>();
        private readonly List<UnnamedPropertyParser> _unnamedParsers = new List<UnnamedPropertyParser>();

        private readonly Type _modelType;
        private readonly ConvertersManager _convertersManager = new ConvertersManager();

        public string CommandName { get; private set; }
        public string CommandDescription { get; private set; }

        public ModelBuilder(Type modelType, string commandName)
        {
            this.CommandName = commandName;
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            this._modelType = modelType;
            this.ValidateModel(modelType);

            this.ReadHelpInformation();
            this.BuildParsingProperties();
        }

        private void BuildParsingProperties()
        {
            var properties = this._modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);
            foreach (var propertyInfo in properties)
            {
                var attributes = propertyInfo.GetCustomAttributes(inherit: true);

                // description attribute

                // syntax attribute

                // other help attribute

                // mandatory attribute
                // TODO: build mandatory list for parsing check-hashmap

                // Is Flag
                CommandFlagAttribute flagAtt = attributes.SingleOrDefault(att => att is CommandFlagAttribute) as CommandFlagAttribute;
                if (flagAtt != null)
                {
                    var parser = new FlagPropertyParser(propertyInfo);

                    foreach (var literal in flagAtt.CommandLiterals)
                    {
                        var aLiteral = literal.Trim();

                        if (this._flagParsers.ContainsKey(aLiteral))
                        {
                            throw new BadImplementationException($"Flag argument literal \"{literal}\" has been declared more than once in the {this._modelType.FullName}.", this._modelType);
                        }

                        this._flagParsers.Add(aLiteral, parser);
                    }
                }

                // Is Switch
                CommandSwitchAttribute switchAttribute = attributes.SingleOrDefault(att => att is CommandSwitchAttribute) as CommandSwitchAttribute;
                if (switchAttribute != null)
                {
                    var parser = this.BuildSwitchParser(propertyInfo, switchAttribute, attributes);
                    if (parser == null)
                    {
                        throw new BadImplementationException($"Could not find proper SwitchParser for property \"{propertyInfo.Name}\"", this._modelType);
                    }

                    foreach (var literal in switchAttribute.CommandLiterals)
                    {
                        var aLiteral = literal.Trim();

                        if (this._switchParsers.ContainsKey(aLiteral))
                        {
                            throw new BadImplementationException($"Flag argument literal \"{literal}\" has been declared more than once in the {this._modelType.FullName}.", this._modelType);
                        }

                        this._switchParsers.Add(aLiteral, parser);
                    }
                }

                // Is Valued Flag - named, free value
                CommandValueFlagAttribute valueAtt = attributes.SingleOrDefault(att => att is CommandValueFlagAttribute) as CommandValueFlagAttribute;
                if (valueAtt != null)
                {
                    var converter = this._convertersManager.GetConverterFor(propertyInfo.PropertyType);
                    if (converter == null)
                    {
                        throw new BadImplementationException($"Could not find required ArgumentConverter for type \"{propertyInfo.PropertyType.FullName}\" for ValueArgument \"{propertyInfo.Name}\".", this._modelType);
                    }

                    var parser = this.BuildValueOptionParser(propertyInfo, valueAtt, attributes, converter);
                    if (parser == null)
                    {
                        throw new BadImplementationException($"Could not find proper SwitchParser for property \"{propertyInfo.Name}\"", this._modelType);
                    }

                    // value parser is mainly compatible with switch-one
                    foreach (var literal in valueAtt.CommandLiterals)
                    {
                        var aLiteral = literal.Trim();

                        if (this._switchParsers.ContainsKey(aLiteral))
                        {
                            throw new BadImplementationException($"Flag argument literal \"{literal}\" has been declared more than once in the {this._modelType.FullName}.", this._modelType);
                        }

                        this._switchParsers.Add(aLiteral, parser);
                    }
                }

                // Is unnamed argument
                CommandUnnamedOptionAttribute nonPosAtt = attributes.SingleOrDefault(att => att is CommandUnnamedOptionAttribute) as CommandUnnamedOptionAttribute;
                if (nonPosAtt != null)
                {
                    var converter = this._convertersManager.GetConverterFor(propertyInfo.PropertyType);
                    if (converter == null)
                    {
                        throw new BadImplementationException($"Could not find required ArgumentConverter for type \"{propertyInfo.PropertyType.FullName}\" for unnamed Argument at index [{nonPosAtt.ArgumentIndex}].", this._modelType);
                    }

                    this._unnamedParsers.Add(new UnnamedPropertyParser(nonPosAtt.ArgumentIndex, propertyInfo, converter));
                }
            }
        }

        private BaseSwitchPropertyParser BuildValueOptionParser(PropertyInfo propertyInfo, CommandValueFlagAttribute valueAtt, object[] attributes, ArgumentConverter converter)
        {
            return new CustomValueSwitchParser(propertyInfo, valueAtt, converter);
        }

        private BaseSwitchPropertyParser BuildSwitchParser(PropertyInfo propertyInfo, CommandSwitchAttribute switchAttribute, object[] otherAttributes)
        {
            if (switchAttribute.SwitchBaseType.IsEnum)
            {
                return new EnumSwitchParser(propertyInfo, switchAttribute);
            }

            return null;
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
            int unnamedIndex = 0;
            string[] flagSwitchPrefixes = options.SwitchCharacters.Concat(options.FlagCharacters).Distinct().ToArray();

            while (argIndex < args.Length)
            {
                string arg = args[argIndex].Trim();

                // need to skip unnamed parameters there are just exactly like one of prefixes (i.e. - single slash or backslash)
                if (flagSwitchPrefixes.All(p => p != arg) &&flagSwitchPrefixes.Any(p => arg.StartsWith(p)))
                {
                    var propertyParser = this.FindProperty(options, arg);
                    if (propertyParser == null)
                    {
                        // TODO: write error about invalid switch
                        return null;
                    }

                    propertyParser.Apply(options, model, args, ref argIndex);
                }
                else
                {
                    var propertyParser = this._unnamedParsers.SingleOrDefault(p => p.ArgumentIndex == unnamedIndex++);
                    if (propertyParser == null)
                    {
                        // TODO: write error about too many unnamed arguments
                        return null;
                    }

                    propertyParser.Apply(options, model, args, ref argIndex);
                }

                argIndex++;
            }

            // TODO: validate mandatory options and unnamed arguments

            return model;
        }

        private BasePropertyParser FindProperty(ICommandParserOptions options, string argSyntax)
        {
            // Flags
            if (!options.AllowFlagsAsOneArgument)
            {
                foreach (var flagPrefix in options.FlagCharacters)
                {
                    string cleanFlag = argSyntax.CutLeftAny(flagPrefix);

                    FlagPropertyParser parser;
                    if (this._flagParsers.TryGetValue(cleanFlag, out parser))
                    {
                        return parser;
                    }
                }
            }
            else
            {
                if (this._flagParsers.Keys.Any(flag => flag.Length > 1))
                {
                    throw new BadImplementationException($"Cannot use {nameof(options.AllowFlagsAsOneArgument)} mode with flags longer than one character.", this._modelType);
                }

                // TODO: implement, with above exception this will be easier...

                // TODO: add multi-flag parser here
            }


            // Switches
            foreach (var switchPrefix in options.SwitchCharacters)
            {
                string cleanFlag = argSyntax.CutLeftAny(switchPrefix);
                BaseSwitchPropertyParser parser = this._switchParsers.FirstOrDefault(p => p.Key.Equals(cleanFlag)).Value;
                return parser;
            }

            return null;
        }


        public void PrintHelp(ICommandOutput output, CommandEngineStyles styles, IEnumerable<string> arguments)
        {
            // TODO: implement
        }
    }
}