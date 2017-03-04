using System;

namespace ConfigServer.Server.Validation
{
    internal class ValidationStrings
    {
        public static string InvalidConfigType(Type type) => $"Configuration is not of type:{type.FullName}";
        public static string LessThanMin(string property, object min) => $"Property: {property} is less than min({min})";
        public static string GreaterThanMax(string property, object max) => $"Property: {property} is greater than max({max})";
        public static string GreaterThanMaxLength(string property, object maxLength)=> $"Property: {property} is greater than max length({maxLength})";
        public static string MatchesPattern(string property, object pattern) => $"Property: {property} does not match pattern({pattern})";
        public static string OptionNotFound(string property) => $"Property: {property} does not match available option";
        public static string DuplicateKeys(string property, object key) => $"Property: {property} has duplicate keys in collection({key})";
        public static string InvalidOptionType(Type type) => $"Configuration is not a collection of type:{type.FullName}";
        public static string DuplicateOptionKeys(string property, object key) => $"Option: {property} has duplicate keys in collection({key})";
        public static string RequiredPropertyNotFound(string property) => $"Property: {property} is required";
    }
}
