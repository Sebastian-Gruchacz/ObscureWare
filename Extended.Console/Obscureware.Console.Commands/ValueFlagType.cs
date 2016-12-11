using System;

namespace Obscureware.Console.Commands
{
    public enum ValueFlagType
    {
        [TargetTypes(typeof(string))]
        Text,

        [TargetTypes(typeof(int), typeof(short), typeof(long), typeof(byte), typeof(uint), typeof(ulong))]
        Integer,

        [TargetTypes(typeof(decimal), typeof(float), typeof(double))]
        Decimal,

        [TargetTypes(typeof(DateTime))]
        DateTime,

        [TargetTypes(typeof(TimeSpan))]
        TimeSpan
    }
}