using Banking.Client.Models;

namespace Banking.Client.Repositories
{
    public interface IClientRepository
    {
        Task<ResponseResult<AddClientResponse>> CreateClientAsync(AddClientRequest client);
    }
}
