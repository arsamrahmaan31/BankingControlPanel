using Banking.Client.ClientLogger;
using Banking.Client.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Banking.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController(IClientManager clientManager, IClientLogger clientLogger, IHttpContextAccessor httpContextAccessor) : Controller
    {
        [HttpPost("AddClient")]
        public async Task<IActionResult> SignUp(AddClientRequest client)
        {
            try
            {

                // Log the request details
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    clientLogger.LogRequest(client, httpContext);
                }

                // Attempt to add client
                var result = await clientManager.SignUpAsync(client);

                // Log the respones and return result
                clientLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUp), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }
    }
}
