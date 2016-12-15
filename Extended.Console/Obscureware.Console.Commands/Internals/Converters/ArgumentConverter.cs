namespace Obscureware.Console.Commands.Internals.Converters
{
    using System.Globalization;

    /// <summary>
    /// Classes dedicated to Argument value conversions.
    /// </summary>
    public abstract class ArgumentConverter
    {
        // WORKAROUND: this boxing is not fine, but we have to live with it... Generics would be a nightmare here...

        /// <summary>
        /// Tries to convert (parse) given string into specific target type.
        /// </summary>
        /// <param name="argumentText"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public abstract object TryConvert(string argumentText, CultureInfo culture);
    }
}