using System.ComponentModel.DataAnnotations;

namespace Banking.Auth.HelperHandlers
{
    public class CompositeValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();

            // Apply role_id validation rule
            if (validationContext.ObjectType.GetProperty("role_id") != null)
            {
                var roleId = (int?)validationContext.ObjectType.GetProperty("role_id").GetValue(validationContext.ObjectInstance);
                if (roleId.HasValue && (roleId.Value != 1 && roleId.Value != 2))
                {
                    validationResults.Add(new ValidationResult("role_id must be either 1 (User) or 2 (Admin)."));
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
    }
}
