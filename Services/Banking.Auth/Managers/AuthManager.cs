using Banking.Auth.Constants;
using Banking.Auth.Models;
using Banking.Auth.Repositories;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Banking.Auth.Managers
{
    public class AuthManager(IAuthRepository authRepository, IConfiguration configuration): IAuthManager
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
                        return new ResponseResult<LoginResponse> { success = true, result = loggedInUser, message = StaticMessages.TokenCreated, token = token };
                    }
                    else
                    {
                        return new ResponseResult<LoginResponse> { success = false, result = new LoginResponse(), message = StaticMessages.IncorrectPassword };
                    }
                }
                else return new ResponseResult<LoginResponse> { success = false, result = new LoginResponse(), message = StaticMessages.UserNotExist };
            }
            catch (Exception ex)
            {
                Log.Error(StaticMessages.ExceptionOccured, nameof(LoginAsync), ex.Message);
                return new ResponseResult<LoginResponse> { success = false, result = new LoginResponse(), message = ex.Message };
            }
        }




        private string CreateToken(string? role_name)
        {

            var claims = new List<Claim>();
            if (role_name != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, role_name));
            }
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                configuration.GetSection(StaticMessages.JwtTokenPath).Value!));

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
