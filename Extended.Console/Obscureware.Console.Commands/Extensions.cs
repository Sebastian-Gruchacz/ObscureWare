namespace Obscureware.Console.Commands
{
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
    }
}
