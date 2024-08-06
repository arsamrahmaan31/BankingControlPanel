using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using System.Net;

namespace Banking.Client.HelperHandlers
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
            if (exception is NotImplementedException || exception is InvalidOperationException)
            {
                statusCode = (int)HttpStatusCode.NotImplemented;
                type = exception.GetType().Name;
                title = StaticMessages.NotImplemented;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is HttpProtocolException || exception is ArgumentNullException || exception is ArgumentOutOfRangeException || exception is FormatException)
            {
                statusCode = (int)HttpStatusCode.BadRequest;
                type = exception.GetType().Name;
                title = StaticMessages.BadRequest;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is NullReferenceException || exception is OverflowException)
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                type = exception.GetType().Name;
                title = StaticMessages.InternalServerError;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is TaskCanceledException)
            {
                statusCode = (int)HttpStatusCode.RequestTimeout;
                type = exception.GetType().Name;
                title = StaticMessages.ServerTimeOut;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else if (exception is UnauthorizedAccessException)
            {
                statusCode = (int)HttpStatusCode.Unauthorized;
                type = exception.GetType().Name;
                title = StaticMessages.UnauthorizedAccess;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;
                type = exception.GetType().Name;
                title = StaticMessages.SomethingWentWrong;
                detail = exception.Message;
                instance = $"{httpContext.Request.Method} {httpContext.Request.Path}";
            }

            Log.Error(exception, StaticMessages.GlobalExceptionOccured, title, statusCode, type, detail, instance);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(new { statusCode, type, title, detail, instance }, cancellationToken);
            return true;
        }
    }
}
