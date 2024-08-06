using Banking.Auth.Models;

namespace Banking.Auth.Managers
{
    public interface IAuthManager
    {
        Task<ResponseResult<LoginResponse>> LoginAsync(LoginRequest login);
    }
}
