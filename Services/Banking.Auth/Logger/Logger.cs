using Banking.Auth.Models;
using Newtonsoft.Json;
using Serilog;

namespace Banking.Auth.Logger
{
    public class Logger : ILogger
    {
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

        public void LogResponse<T>(ResponseResult<T> response)
        {
            var serializedResponse = JsonConvert.SerializeObject(response);
            Log.Information("Response Object is: {SerializedResponse}", serializedResponse);
        }
    }
}
