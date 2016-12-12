namespace Obscureware.Console.Commands.Model
{
    using System;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TargetTypesAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        public TargetTypesAttribute(params Type[] targetTypes)
        {
            this.TargetTypes = targetTypes;
        }
    }
}