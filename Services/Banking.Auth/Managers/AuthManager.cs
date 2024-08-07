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
                var credentialsFound = await authRepository.IsLoginExistsAsync(login.email);
                if (credentialsFound != null)
                {
                    if (BCrypt.Net.BCrypt.Verify(login.password, credentialsFound.password))
                    {
                        string token = string.Empty;
                        if (credentialsFound.role_id != null && credentialsFound.email != null)
                        {
                            token = CreateToken(credentialsFound.role_name);
                        }
                        var loggedInUser = new LoginResponse
                        {
                            first_name = credentialsFound.first_name,
                            last_name = credentialsFound.last_name,
                            email = credentialsFound.email,
                            role_name = credentialsFound.role_name,
                        };
                        return new ResponseResult<LoginResponse>
                        {
                            success = true,
                            status_code = (int)HttpStatusCode.OK,
                            result = loggedInUser,
                            message = StaticMessages.TokenCreated,
                            token = token
                        };
                    }
                    else
                    {
                        return new ResponseResult<LoginResponse>
                        {
                            success = false,
                            status_code = (int)HttpStatusCode.Unauthorized,
                            result = new LoginResponse(),
                            message = StaticMessages.IncorrectPassword
                        };
                    }
                }
                else return new ResponseResult<LoginResponse>
                {
                    success = false,
                    status_code = (int)HttpStatusCode.NotFound,
                    result = new LoginResponse(),
                    message = StaticMessages.UserNotExist
                };
            }
            catch (Exception ex)
            {
                Log.Error(StaticMessages.ExceptionOccured, nameof(LoginAsync), ex.Message);
                return new ResponseResult<LoginResponse> { success = false, status_code = (int)HttpStatusCode.InternalServerError, result = new LoginResponse(), message = ex.Message };
            }
        }

        public async Task<ResponseResult<SignUpResponse>> SignUpAsync(SignUpRequest user)
        {
            try
            {
                // Check if user email already exists
                var credentialsFound = await authRepository.IsLoginExistsAsync(user.email);
                if (credentialsFound != null)
                {
                    return new ResponseResult<SignUpResponse>
                    {
                        success = false,
                        status_code = (int)HttpStatusCode.Conflict,
                        result = null,
                        message = StaticMessages.UserAlreadyExist
                    };
                }

                // Hash the password and set it
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(user.password);
                user.password = passwordHash;

                // Create user in the database
                ResponseResult<SignUpResponse> createUserResult = await authRepository.SignUpAsync(user);

                if (createUserResult.success)
                {
                    return new ResponseResult<SignUpResponse>
                    {
                        success = true,
                        status_code= (int)HttpStatusCode.OK,
                        result = createUserResult.result,
                        message = StaticMessages.UserCreated
                    };
                }
                else
                {
                    return new ResponseResult<SignUpResponse>
                    {
                        success = false,
                        result = null,
                        status_code=(int)HttpStatusCode.InternalServerError,
                        message = StaticMessages.SomethingWentWrong
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), ex.Message);
                return new ResponseResult<SignUpResponse>
                {
                    success = false,
                    status_code = (int)HttpStatusCode.InternalServerError,
                    result = null,
                    message = ex.Message
                };
            }
        }


        public string CreateToken(string? role_name)
        {

            var claims = new List<Claim>();
            if (role_name != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role_name));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

    }
}
