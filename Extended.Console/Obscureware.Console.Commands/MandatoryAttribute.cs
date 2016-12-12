using System;

namespace Obscureware.Console.Commands
{
    public class MandatoryAttribute : Attribute
    {
        public bool IsParameterMandatory { get; private set; }

        public MandatoryAttribute(bool isParameterMandatory = true)
        {
            IsParameterMandatory = isParameterMandatory;
        }
    }
}