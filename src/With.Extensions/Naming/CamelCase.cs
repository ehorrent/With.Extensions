using System.Text;

namespace With.Naming
{
    /// <summary>
    /// Camel case conventions
    /// </summary>
    public static class CamelCase
    {
        /// <summary>
        /// Rough version converting first character to capital letter.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Corresponding camel case value</returns>
        public static string Convert(string value)
        {
            var builder = new StringBuilder(value);
            builder[0] = char.ToUpper(builder[0]);
            return builder.ToString();
        }
    }
}
