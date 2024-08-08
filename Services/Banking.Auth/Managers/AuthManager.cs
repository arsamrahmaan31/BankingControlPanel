using Banking.Auth.Constants;
using Banking.Auth.Models;
using Banking.Auth.Repositories;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Banking.Auth.Managers
{
    public class AuthManager(IAuthRepository authRepository, IConfiguration configuration) : IAuthManager
    {
        public async Task<ResponseResult<LoginResponse>> LoginAsync(LoginRequest login)
        {
            try
            {
                // Check if the user credentials exist in the system
                var credentialsFound = await authRepository.IsLoginExistsAsync(login.email);
                if (credentialsFound != null)
                {
                    // Verify the provided password against the stored hash
                    if (BCrypt.Net.BCrypt.Verify(login.password, credentialsFound.password))
                    {
                        string token = string.Empty;

                        // Generate a token if role_id and email are available
                        if (credentialsFound.role_id != null && credentialsFound.email != null)
                        {
                            token = CreateToken(credentialsFound.role_name);
                        }

                        // Create the response object for a successful login
                        var loggedInUser = new LoginResponse
                        {
                            user_id = credentialsFound.user_id,
                            first_name = credentialsFound.first_name,
                            last_name = credentialsFound.last_name,
                            email = credentialsFound.email,
                            role_name = credentialsFound.role_name,
                        };

                        // Return success response with user details and token
                        return new ResponseResult<LoginResponse>
                        {
                            success = true,status_code = (int)HttpStatusCode.OK, result = loggedInUser,message = StaticMessages.TokenCreated, token = token
                        };
                    }
                    else
                    {
                        // Return unauthorized response if password verification fails
                        return new ResponseResult<LoginResponse>
                        {
                            success = false,status_code = (int)HttpStatusCode.Unauthorized,result = new LoginResponse(),message = StaticMessages.IncorrectPassword
                        };
                    }
                }

                // Return not found response if user credentials do not exist
                else return new ResponseResult<LoginResponse>
                {
                    success = false,status_code = (int)HttpStatusCode.NotFound,result = new LoginResponse(),message = StaticMessages.UserNotExist
                };
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow the exception handled by global exception middleware
                Log.Error(StaticMessages.ExceptionOccured, nameof(LoginAsync), ex.Message);
                throw;
            }
        }

        public async Task<ResponseResult<SignUpResponse>> SignUpAsync(SignUpRequest user)
        {
            try
            {
                // Check if the user email already exists in the system
                var credentialsFound = await authRepository.IsLoginExistsAsync(user.email);
                if (credentialsFound != null)
                {
                    // Return conflict response if the email already exists
                    return new ResponseResult<SignUpResponse>
                    {
                        success = false,status_code = (int)HttpStatusCode.Conflict,result = null,message = StaticMessages.UserAlreadyExist
                    };
                }

                // Hash the user's password for security
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                user.password = passwordHash;

                // Attempt to create the user in the database
                ResponseResult<SignUpResponse> createUserResult = await authRepository.SignUpAsync(user);

                if (createUserResult.success)
                {
                    // Return success response if user creation is successful
                    return new ResponseResult<SignUpResponse>
                    {
                        success = true,status_code= (int)HttpStatusCode.OK,result = createUserResult.result,message = StaticMessages.UserCreated
                    };
                }
                else
                {
                    // Return internal server error response if user creation fails
                    return new ResponseResult<SignUpResponse>
                    {
                        success = false, result = null,status_code=(int)HttpStatusCode.InternalServerError,message = StaticMessages.SomethingWentWrong
                    };
                }
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow the exception handled by global exception middleware
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), ex.Message);
                throw;
            }
        }


        private string CreateToken(string? role_name)
        {
            // Initialize the list of claims
            var claims = new List<Claim>();

            // Add role claim if role_name is provided
            if (role_name != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role_name));
            }

            // Create a symmetric security key using the secret from configuration
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection(SystemConstants.JwtTokenPath).Value!));

            // Define the signing credentials using the HMAC SHA-512 algorithm
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create a JWT token with the claims and an expiration of 1 day
            var token = new JwtSecurityToken(claims: claims, expires: DateTime.Now.AddDays(1), signingCredentials: credentials);

            // Serialize the token to a string and return it
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
