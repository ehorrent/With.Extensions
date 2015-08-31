using System.Text;

namespace With.Naming
{
    /// <summary>
    /// Pascal case conventions
    /// </summary>
    public static class PascalCase
    {
        /// <summary>
        /// Rough version converting first character to capital letter.
        /// </summary>
        /// <param name="value">Value to convert</param>
        /// <returns>Corresponding pascal case value</returns>
        public static string Convert(string value)
        {
            var builder = new StringBuilder(value);
            builder[0] = char.ToUpper(builder[0]);
            return builder.ToString();
        }
    }
}
