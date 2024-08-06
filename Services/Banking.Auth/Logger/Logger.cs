using Banking.Auth.Models;
using Newtonsoft.Json;
using Serilog;

namespace Banking.Auth.Logger
{
    public class Logger : ILogger
    {
        /// <summary>
        /// Logs details about an incoming request, including IP address, request method, path, user agent, and the request object.
        /// </summary>
        /// <param name="dto">The data transfer object representing the request payload.</param>
        /// <param name="httpContext">The HTTP context containing request details.</param>
        public void LogRequest(object dto, HttpContext httpContext)
        {
            var ipAddress = httpContext?.Connection.RemoteIpAddress;
            var dateOfRequestGenerated = DateTime.Now;
            var requestMethod = httpContext?.Request.Method;
            var requestPath = httpContext?.Request.Path;
            var userAgent = httpContext?.Request.Headers.UserAgent;

            var serializedDto = JsonConvert.SerializeObject(dto);

            Log.Information("Request is: Controller {ControllerName} called with IP {IpAddress} at {DateOfRequest}. Request details: Method: {RequestMethod}, Path: {RequestPath}, User Agent: {UserAgent}. Request Object is: {SerializedDto}.",
                "AuthController", ipAddress, dateOfRequestGenerated, requestMethod, requestPath, userAgent, serializedDto);
        }

        /// <summary>
        /// Logs the details of the response object.
        /// </summary>
        /// <param name="response">The response object to be logged.</param>
        /// <typeparam name="T">The type of the response object.</typeparam>
        public void LogResponse<T>(ResponseResult<T> response)
        {
            var serializedResponse = JsonConvert.SerializeObject(response);
            Log.Information("Response Object is: {SerializedResponse}", serializedResponse);
        }
    }
}
