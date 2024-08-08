using Banking.Auth.Constants;
using Banking.Auth.Logger;
using Banking.Auth.Managers;
using Banking.Auth.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Reflection.Metadata;

namespace Banking.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthManager _authManager;
        private readonly IAuthLogger _authLogger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(IAuthManager authManager, IAuthLogger authLogger, IHttpContextAccessor httpContextAccessor)
        {
            _authManager = authManager;
            _authLogger = authLogger;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Signin([FromBody] LoginRequest login)
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
                    _authLogger.LogRequest(login, httpContext);
                }

                // Attempt to log in
                var result = await _authManager.LoginAsync(login);

                // Handle failed login attempt
                if (!result.success)
                {
                    _authLogger.LogResponse(result);
                    return Unauthorized(result);
                }

                // Log successful response and return result
                _authLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(Signin), ex.Message);

                // The exception will be handled by the global exception handler middleware
                throw;
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> CreateUser(SignUpRequest user)
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
                    _authLogger.LogRequest(user, httpContext);
                }

                // Attempt to sign up
                var result = await _authManager.SignUpAsync(user);

                // Log the respones and return result
                _authLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Log.Error(StaticMessages.ExceptionOccured, nameof(CreateUser), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }
    }
}
