using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ConfigServer.Server.Validation
{
    internal interface IConfigurationValidator
    {
        ValidationResult Validate(object target, ConfigurationModel model);
    }

    internal class ConfigurationValidator : IConfigurationValidator
    {
        private readonly IOptionSetFactory optionSetFactory;

        public ConfigurationValidator(IOptionSetFactory optionSetFactory)
        {
            this.optionSetFactory = optionSetFactory;
        }

        public ValidationResult Validate(object target, ConfigurationModel model)
        {
            if (target == null || target.GetType() != model.Type)
                return new ValidationResult(string.Format(ValidationStrings.InvalidConfigType, model.Type.FullName));
            return ValidateProperties(target, model.ConfigurationProperties);
        }

        private ValidationResult ValidateProperties(object target, Dictionary<string, ConfigurationPropertyModelBase> properties)
        {
            var propertyResults = properties .Select(propModel => ValidateProperty(target, propModel.Value));
            return new ValidationResult(propertyResults);
        }
        private ValidationResult ValidateProperty(object target, ConfigurationPropertyModelBase propertyModelBase)
        {
            switch (propertyModelBase)
            {
                case ConfigurationPrimitivePropertyModel propertyModel:
                    return ValidateProperty(target, propertyModel);
                case ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel:
                    return ValidateProperty(target, propertyModel);
                case ConfigurationPropertyWithOptionsModelDefinition propertyModel:
                    return ValidateProperty(target, propertyModel);
                case ConfigurationCollectionPropertyDefinition propertyModel:
                    return ValidateProperty(target, propertyModel);
                default:
                    return ValidationResult.CreateValid();
            }
        }

        private ValidationResult ValidateProperty(object target, ConfigurationPrimitivePropertyModel propertyModel)
        {
            var errors = new List<string>();
            var propertyValue = propertyModel.GetPropertyValue(target);
            if (propertyModel.ValidationRules.Min != null && propertyModel.ValidationRules.Min.CompareTo(propertyValue) > 0)
                errors.Add(string.Format(ValidationStrings.LessThanMin, propertyModel.ConfigurationPropertyName, propertyModel.ValidationRules.Min));
            if (propertyModel.ValidationRules.Max != null && propertyModel.ValidationRules.Max.CompareTo(propertyValue) < 0)
                errors.Add(string.Format(ValidationStrings.GreaterThanMax, propertyModel.ConfigurationPropertyName, propertyModel.ValidationRules.Max));
            if (propertyModel.ValidationRules.MaxLength.HasValue && propertyValue is string && ((string)propertyValue).Length > propertyModel.ValidationRules.MaxLength)
                errors.Add(string.Format(ValidationStrings.GreaterThanMaxLength, propertyModel.ConfigurationPropertyName, propertyModel.ValidationRules.MaxLength));
            if (!string.IsNullOrWhiteSpace(propertyModel.ValidationRules.Pattern) && propertyValue is string && !Regex.IsMatch((string)propertyValue, propertyModel.ValidationRules.Pattern))
                errors.Add(string.Format(ValidationStrings.MatchesPattern, propertyModel.ConfigurationPropertyName, propertyModel.ValidationRules.Pattern));
            return new ValidationResult(errors);
        }

        private ValidationResult ValidateProperty(object target, ConfigurationPropertyWithOptionsModelDefinition propertyModel)
        {
            var errors = new List<string>();
            var propertyValue = propertyModel.GetPropertyValue(target);
            var options = optionSetFactory.Build(propertyModel);
            if(!options.OptionKeyInSet(propertyValue))
                errors.Add(string.Format(ValidationStrings.OptionNotFound, propertyModel.ConfigurationPropertyName));
            return new ValidationResult(errors);
        }

        private ValidationResult ValidateProperty(object target, ConfigurationPropertyWithMultipleOptionsModelDefinition propertyModel)
        {
            var errors = new List<string>();
            var propertyValue = propertyModel.GetPropertyValue(target) as IEnumerable;
            var options = optionSetFactory.Build(propertyModel);
            foreach(var value in propertyValue)
            {
                if (options.OptionKeyInSet(value))
                    continue;
                errors.Add(string.Format(ValidationStrings.OptionNotFound, propertyModel.ConfigurationPropertyName));
                break;
            }
            return new ValidationResult(errors);
        }

        private ValidationResult ValidateProperty(object target, ConfigurationCollectionPropertyDefinition propertyModel)
        {
            var results = new List<ValidationResult>();
            var propertyValue = propertyModel.GetPropertyValue(target) as IEnumerable;
            var duplicateChecker = new HashSet<string>();
            foreach (var value in propertyValue)
            {
                if (propertyModel.HasUniqueKey)
                {
                    var key = propertyModel.GetKeyFromMember(value);
                    if (!duplicateChecker.Add(key))
                        results.Add(new ValidationResult(string.Format(ValidationStrings.DuplicateKeys, propertyModel.ConfigurationPropertyName, key)));
                }
                results.Add(ValidateProperties(value, propertyModel.ConfigurationProperties));
            }
            return new ValidationResult(results);
        }
    }
}
