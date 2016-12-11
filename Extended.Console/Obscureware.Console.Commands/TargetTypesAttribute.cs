using System;

namespace Obscureware.Console.Commands
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class TargetTypesAttribute : Attribute
    {
        public Type[] TargetTypes { get; private set; }

        public TargetTypesAttribute(params Type[] targetTypes)
        {
            TargetTypes = targetTypes;
        }
    }
}