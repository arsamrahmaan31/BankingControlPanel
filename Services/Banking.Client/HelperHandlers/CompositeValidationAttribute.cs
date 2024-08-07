using Banking.Client.Models;
using PhoneNumbers;
using System.ComponentModel.DataAnnotations;

namespace Banking.Client.HelperHandlers
{
    public class CompositeValidationAttribute : ValidationAttribute
    {

        private static readonly PhoneNumberUtil phoneNumberUtil = PhoneNumberUtil.GetInstance();
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();



            // Validate mobile_number
            if (validationContext.ObjectType.GetProperty("mobile_number") != null)
            {
                var mobileNumber = (string)validationContext.ObjectType.GetProperty("mobile_number").GetValue(validationContext.ObjectInstance);
                if (!IsValidPhoneNumber(mobileNumber))
                {
                    validationResults.Add(new ValidationResult("Mobile number format is not valid."));
                }
            }

            // Combine all validation results into a single result
            if (validationResults.Any())
            {
                // Create a single ValidationResult with all errors
                var combinedErrorMessage = string.Join("; ", validationResults.Select(r => r.ErrorMessage));
                return new ValidationResult(combinedErrorMessage);
            }

            return ValidationResult.Success;
        }

        // Validate phone number using libphonenumber-csharp
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            try
            {
                var phoneNumberProto = phoneNumberUtil.Parse(phoneNumber, null);
                return phoneNumberUtil.IsValidNumber(phoneNumberProto);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
