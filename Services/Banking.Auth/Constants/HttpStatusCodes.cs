namespace Banking.Auth.Constants
{
    public static class HttpStatusCodes
    {
        // Success codes
        public const int OK = 200;
        public const int Created = 201;

        // Error codes
        public const int BadRequest = 400;
        public const int Unauthorized = 401;
        public const int Forbidden = 403;
        public const int NotFound = 404;
        public const int MethodNotAllowed = 405;

        // Server error codes
        public const int InternalServerError = 500;
        public const int NotImplemented = 501;
        public const int BadGateway = 502;
    }

}
