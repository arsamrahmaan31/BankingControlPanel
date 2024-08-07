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


            // Validate email
            if (validationContext.ObjectType.GetProperty("email") != null)
            {
                var email = (string)validationContext.ObjectType.GetProperty("email").GetValue(validationContext.ObjectInstance);
                if (string.IsNullOrWhiteSpace(email))
                {
                    validationResults.Add(new ValidationResult("email is required."));
                }
                else if (!new EmailAddressAttribute().IsValid(email))
                {
                    validationResults.Add(new ValidationResult("email format is not valid."));
                }
            }

            // Validate first_name
            if (validationContext.ObjectType.GetProperty("first_name") != null)
            {
                var firstName = (string)validationContext.ObjectType.GetProperty("first_name").GetValue(validationContext.ObjectInstance);
                if (string.IsNullOrWhiteSpace(firstName))
                {
                    validationResults.Add(new ValidationResult("first_name is required."));
                }
                else if (firstName.Length >= 60)
                {
                    validationResults.Add(new ValidationResult("first_name must be less than 60 characters."));
                }
            }

            // Validate LastName
            if (validationContext.ObjectType.GetProperty("last_name") != null)
            {
                var lastName = (string)validationContext.ObjectType.GetProperty("last_name").GetValue(validationContext.ObjectInstance);
                if (string.IsNullOrWhiteSpace(lastName))
                {
                    validationResults.Add(new ValidationResult("last_name is required."));
                }
                else if (lastName.Length >= 60)
                {
                    validationResults.Add(new ValidationResult("last_name must be less than 60 characters."));
                }
            }

            // Validate mobile_number
            if (validationContext.ObjectType.GetProperty("mobile_number") != null)
            {
                var mobileNumber = (string)validationContext.ObjectType.GetProperty("mobile_number").GetValue(validationContext.ObjectInstance);
                if (string.IsNullOrWhiteSpace(mobileNumber))
                {
                    validationResults.Add(new ValidationResult("Mobile number is required."));
                }
                else if (!IsValidPhoneNumber(mobileNumber))
                {
                    validationResults.Add(new ValidationResult("Mobile number format is not valid."));
                }
            }

            // Validate personal_id
            if (validationContext.ObjectType.GetProperty("personal_id") != null)
            {
                var personalId = (string)validationContext.ObjectType.GetProperty("personal_id").GetValue(validationContext.ObjectInstance);
                if (string.IsNullOrWhiteSpace(personalId))
                {
                    validationResults.Add(new ValidationResult("personal_id id is required."));
                }
                else if (personalId.Length != 11)
                {
                    validationResults.Add(new ValidationResult("personal_id must be exactly 11 characters."));
                }
            }


            // Validate gender_id
            if (validationContext.ObjectType.GetProperty("gender_id") != null)
            {
                var roleId = (int?)validationContext.ObjectType.GetProperty("gender_id").GetValue(validationContext.ObjectInstance);
                if (roleId.HasValue && (roleId.Value != 1 && roleId.Value != 2))
                {
                    validationResults.Add(new ValidationResult("gender_id must be either 1 (Male) or 2 (Female)."));
                }
            }

            // Validate client_accounts
            var clientAccountsProperty = validationContext.ObjectType.GetProperty("client_accounts");
            if (clientAccountsProperty != null)
            {
                var accounts = clientAccountsProperty.GetValue(validationContext.ObjectInstance) as List<ClientAccount>;
                if (accounts == null || !accounts.Any() )
                {
                    validationResults.Add(new ValidationResult("At least one client_account is required."));
                }
                else
                {
                    // Check each account's account_number for non-empty value
                    foreach (var account in accounts)
                    {
                        if (string.IsNullOrWhiteSpace(account.account_number))
                        {
                            validationResults.Add(new ValidationResult("Each client_account must have a non-empty account_number."));
                            break; // Stop checking after finding the first invalid account
                        }
                    }
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
