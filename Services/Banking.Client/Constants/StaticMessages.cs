namespace Banking.Client.Constants
{
    public static class StaticMessages
    {
        // General messages
        public const string NotFound = "The requested resource was not found.";
        public const string BadRequest = "Bad Request.";
        public const string NotImplemented = "Not Implemented.";

        // User messages
        public const string UserCreated = "User has been created successfully.";
        public const string TokenCreated = "Token has been generated successfully.";

        // Client message
        public const string ClientAdded = "Client has been added successfully.";

        // Error messages
        public const string InvalidInput = "The input provided is invalid.";
        public const string UnauthorizedAccess = "You do not have permission to access this resource.";
        public const string ServerTimeOut = "Server timeout occurs due to a slow client request.";
        public const string UnexpectedError = "An unexpected error occurred. Please contact support.";
        public const string InternalServerError = "Internal Server Error.";
        public const string SomethingWentWrong = "Something went wrong.";
        public const string IncorrectPassword = "The entered password is incorrect.";
        public const string UserNotExist = "User with this email does not exist.";
        public const string UserAlreadyExist = "User with this email already exist.";

        // Validation messages
        public const string EmailRequired = "Email address is required.";
        public const string PasswordRequired = "Password is required.";
        public const string InvalidEmailFormat = "The email address format is invalid.";

        // Exception message
        public const string ExceptionOccured = "An exception has occured. Details:: Method name: {MethodName}, Error message: {ErrorMessage}";
        public const string GlobalExceptionOccured = "Hello! An Unhandled exception occurred: {title}, statuscode: {statuscode}, type: {type}, detail: {detail}, instance: {instance}";

        // Database Error
        public const string DatabaseErrorOccured = "A database error occurred. Please try again later.";
        public const string DatabaseNotConfigured = "Database connection string is not configured.";
    }
}
