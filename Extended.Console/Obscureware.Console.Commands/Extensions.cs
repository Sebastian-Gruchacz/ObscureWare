namespace Obscureware.Console.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.ComTypes;

    public static class Extensions
    {
        /// <summary>
        /// Returns base text without first matched prefix. Matching from the left side of the text.
        /// </summary>
        /// <param name="baseText"></param>
        /// <param name="prefixes"></param>
        /// <returns></returns>
        public static string CutLeftAny(this string baseText, params string[] prefixes)
        {
            foreach (string prefix in prefixes)
            {
                if (baseText.StartsWith(prefix))
                {
                    return baseText.Substring(prefix.Length);
                }
            }

            return baseText;
        }

        private static readonly Random Rnd = new Random();

        /// <summary>
        /// Picks one element from collection randomly.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static T SelectRandom<T>(this ICollection<T> collection)
        {
            int index = Rnd.Next(0, collection.Count);
            return collection.Skip(index).First();
        }
    }
}
