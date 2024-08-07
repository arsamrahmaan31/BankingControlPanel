using Banking.Client.Models;

namespace Banking.Client.Managers
{
    public interface IClientManager
    {
        Task<ResponseResult<AddClientResponse>> AddClientAsync(AddClientRequest client);
    }
}
