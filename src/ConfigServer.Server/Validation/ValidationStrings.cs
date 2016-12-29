namespace ConfigServer.Server.Validation
{
    internal class ValidationStrings
    {
        public const string InvalidConfigType = "Configuration is not of type:{0}";
        public const string LessThanMin = "Property: {0} is less than min({1})";
        public const string GreaterThanMax = "Property: {0} is greater than max({1})";
        public const string GreaterThanMaxLength = "Property: {0} is greater than max length({1})";
        public const string MatchesPattern = "Property: {0} does not match pattern({1})";
        public const string OptionNotFound = "Property: {0} does not match available option";
    }
}
