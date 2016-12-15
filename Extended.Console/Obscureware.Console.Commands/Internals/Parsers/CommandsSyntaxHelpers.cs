namespace Obscureware.Console.Commands.Internals.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Common helper functions
    /// </summary>
    internal static class CommandsSyntaxHelpers
    {
        public static IEnumerable<T> Combine<T>(IEnumerable<T> prefixes, IEnumerable<T> roots, Func<T,T,T> mergeFunc)
        {
            return roots.SelectMany(r => prefixes.Select(p => mergeFunc(p, r)));
        }
    }
}