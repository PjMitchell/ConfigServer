namespace ConfigServer.Server
{
    internal static class PropertyNameParser
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled);
        }

        public static string ToLowerCamelCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return input;
            var firstChar = char.ToLower(input[0]);
            return input.Length >1? firstChar + input.Substring(1): firstChar.ToString();
        }
    }
}
