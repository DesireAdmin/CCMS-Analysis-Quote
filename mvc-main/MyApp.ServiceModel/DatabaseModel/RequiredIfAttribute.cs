using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class RequiredIfAttribute : ValidationAttribute
    {
        private readonly string _booleanPropertyName;
        private readonly bool _expectedValue;

        public RequiredIfAttribute(string booleanPropertyName, bool expectedValue)
        {
            _booleanPropertyName = booleanPropertyName;
            _expectedValue = expectedValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the boolean property and its value
            var booleanProperty = validationContext.ObjectType.GetProperty(_booleanPropertyName);
            if (booleanProperty == null)
                return new ValidationResult($"Property {_booleanPropertyName} does not exist.");

            var booleanValue = (bool)booleanProperty.GetValue(validationContext.ObjectInstance);

            // If the boolean value matches the expected value and the field is empty, return validation error
            if (booleanValue == _expectedValue && value == null)
            {
                return new ValidationResult($"{validationContext.DisplayName} is required.");
            }

            return ValidationResult.Success;
        }
    }
}
