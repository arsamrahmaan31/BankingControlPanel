using Banking.Auth.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;

namespace Banking.Auth.HelperClasses
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            int statusCode;
            string type;
            string title;
            string detail;
            string instance;

            // Handle specific exceptions and map them to corresponding HTTP status codes and messages
            if (exception is NotImplementedException || exception is InvalidOperationException)
            {
                // 501 Not Implemented or 500 Internal Server Error for NotImplementedException and InvalidOperationException
                statusCode = (int)HttpStatusCode.NotImplemented;
                type = exception.GetType().Name;
                title = StaticMessages.NotImplemented;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is HttpProtocolException || exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is FormatException)
            {
                // 400 Bad Request for HttpProtocolException, ArgumentNullException, ArgumentOutOfRangeException, and FormatException
                statusCode = (int)HttpStatusCode.BadRequest;
                type = exception.GetType().Name;
                title = StaticMessages.BadRequest;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is NullReferenceException || exception is OverflowException)
            {
                // 500 Internal Server Error for NullReferenceException and OverflowException
                statusCode = (int)HttpStatusCode.InternalServerError;
                type = exception.GetType().Name;
                title = StaticMessages.InternalServerError;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is TaskCanceledException)
            {
                // 408 Request Timeout for TaskCanceledException
                statusCode = (int)HttpStatusCode.RequestTimeout;
                type = exception.GetType().Name;
                title = StaticMessages.ServerTimeOut;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is UnauthorizedAccessException)
            {
                // 401 Unauthorized for UnauthorizedAccessException
                statusCode = (int)HttpStatusCode.Unauthorized;
                type = exception.GetType().Name;
                title = StaticMessages.UnauthorizedAccess;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else
            {
                // Default to 500 Internal Server Error for all other exceptions
                statusCode = (int)HttpStatusCode.InternalServerError;
                type = exception.GetType().Name;
                title = StaticMessages.SomethingWentWrong;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }

            // Log the exception details
            Log.Error(exception, StaticMessages.GlobalExceptionOccured, title, statusCode, type, detail, instance);

            // Set response content type and status code
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;

            // Write the error details to the API response
            await httpContext.Response.WriteAsJsonAsync(new { statusCode, type, title, detail, instance }, cancellationToken);
            return true;
        }
    }
}
