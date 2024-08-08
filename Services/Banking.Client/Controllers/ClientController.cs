using Banking.Client.ClientLogger;
using Banking.Client.Constants;
using Banking.Client.Managers;
using Banking.Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Banking.Client.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : Controller
    {
        private readonly IClientManager _clientManager;
        private readonly IClientLogger _clientLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
        public ClientController(IClientManager clientManager,IClientLogger clientLogger,IHttpContextAccessor httpContextAccessor)
        {
            _clientManager = clientManager;
            _clientLogger = clientLogger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("AddNewClient")]
        public async Task<IActionResult> AddClient([FromForm] AddClientRequest client)
        {
            try
            {
                // Validate the model
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Log the request details
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    _clientLogger.LogRequest(client, httpContext);
                }

                // Attempt to add client
                var result = await _clientManager.AddClientAsync(client);

                // Log the respones and return result
                _clientLogger.LogResponse(result);
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

        [HttpGet("GetAllClientsRecord")]
        public async Task<IActionResult> GetAllClients(int loggedIn_user_id, [FromQuery]QueryParameters queryParameters)
        {
            try
            {
                // Valid Id check
                if (loggedIn_user_id == 0)
                {
                    return BadRequest(StaticMessages.InValidUserId);
                }

                // Log the request details
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    _clientLogger.LogRequest(queryParameters, httpContext);
                }

                // Attempt to add client
                var result = await _clientManager.GetClientsAsync(loggedIn_user_id, queryParameters);

                // Log the respones and return result
                _clientLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetAllClients), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }

        [HttpGet("GetLastThreeSuggestions")]
        public async Task<IActionResult> GetLastestSearchSuggestions(int loggedIn_user_id)
        {
            try
            {
                // Valid Id check
                if (loggedIn_user_id == 0)
                {
                    return BadRequest(StaticMessages.InValidUserId);
                }

                // Log the request details
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    _clientLogger.LogRequest(loggedIn_user_id, httpContext);
                }

                // Attempt to get suggesstions
                var result = await _clientManager.GetLatestSearchAsync(loggedIn_user_id);

                // Log the respones and return result
                _clientLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetLastestSearchSuggestions), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }


    }
}
