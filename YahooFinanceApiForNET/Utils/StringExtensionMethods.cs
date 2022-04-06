using System.Text;

namespace YahooFinanceApiForNET.Utils
{
    internal static class StringExtensionMethods
    {
        /// <summary>
        /// Indicates wheter a specified string is null, empty or consist only of white-space characters.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>true if the value of the string is null or string.Empty or if value consist only of white-space characters</returns>
        internal static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        internal static string ParseSymbolsIntoString(this IEnumerable<string> symbols)
        {
            string str = string.Empty;

            foreach (string symbol in symbols)
            {
                if (symbol == symbols.Last())
                {
                    str = $"{str}{symbol}";
                    continue;
                }

                str = $"{str}{symbol},";
            }

            return str;
        }
    }
}
