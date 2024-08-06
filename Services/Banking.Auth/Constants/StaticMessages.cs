namespace Banking.Auth.Constants
{
    public static class StaticMessages
    {
        // General messages
        public const string NotFound = "The requested resource was not found.";

        // User messages
        public const string UserCreated = "User has been created successfully.";

        // Error messages
        public const string InvalidInput = "The input provided is invalid.";
        public const string UnauthorizedAccess = "You do not have permission to access this resource.";
        public const string UnexpectedError = "An unexpected error occurred. Please contact support.";

        // Validation messages
        public const string EmailRequired = "Email address is required.";
        public const string PasswordRequired = "Password is required.";
        public const string InvalidEmailFormat = "The email address format is invalid.";

    }

}
