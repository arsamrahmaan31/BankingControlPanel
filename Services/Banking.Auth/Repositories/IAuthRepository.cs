using Banking.Auth.Models;

namespace Banking.Auth.Repositories
{
    public interface IAuthRepository
    {
        Task<UserVerificationResult> IsLoginExistsAsync(string email);
    }
}
