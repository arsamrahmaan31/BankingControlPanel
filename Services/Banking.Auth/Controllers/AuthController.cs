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
    public class AuthController(IAuthManager authManager, IAuthLogger authLogger, IHttpContextAccessor httpContextAccessor) : Controller
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest login)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Log the body of HTTP request
                    authLogger.LogRequest(login, httpContext);
                }
                var result = await authManager.LoginAsync(login);

                // Log the failed login attempt and return an Unauthorized response
                if (!result.success)
                {
                    authLogger.LogResponse(result);
                    return Unauthorized(result);
                }

                // Log the respones and return result
                authLogger.LogResponse(result);
                return Ok(result);
            }

            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(Login), ex.Message);
                
                // The exception will automatically be handled by "GlobalExceptionHandler" middleware
                throw;
            }
        }

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp(SignUpRequest user)
        {
            try
            {
                var httpContext = httpContextAccessor.HttpContext;
                if (httpContext != null)
                {
                    // Log the body of HTTP request
                    authLogger.LogRequest(user, httpContext);
                }
                var result = await authManager.SignUpAsync(user);

                // Log the respones and return result
                authLogger.LogResponse(result);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUp), ex.Message);

                // The exception will automatically be handled by "GlobalExceptionHandler" middleware
                throw;
            }
        }
    }
}
