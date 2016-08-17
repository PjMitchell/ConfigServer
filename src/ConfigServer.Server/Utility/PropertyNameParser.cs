namespace ConfigServer.Server
{
    internal class PropertyNameParser
    {
        public static string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled);
        }
    }
}
