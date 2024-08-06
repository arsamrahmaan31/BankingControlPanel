using Banking.Client.ClientLogger;
using Banking.Client.Constants;
using Banking.Client.Managers;
using Banking.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Banking.Client.Controllers
{
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController(IClientManager clientManager, IClientLogger clientLogger, IHttpContextAccessor httpContextAccessor) : Controller
    {
        [HttpPost("AddClient")]
        public async Task<IActionResult> AddClient(AddClientRequest client)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Log the request details
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    clientLogger.LogRequest(client, httpContext);
                }

                // Attempt to add client
                var result = await clientManager.AddClientAsync(client);

                // Log the respones and return result
                clientLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddClient), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }
    }
}
