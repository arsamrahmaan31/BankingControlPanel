using Banking.Client.Models;

namespace Banking.Client.Managers
{
    public interface IClientManager
    {
        Task<ResponseResult<AddClientResponse>> AddClientAsync(AddClientRequest client);
        Task<ResponseResult<List<Clients>>> GetClientsAsync(int loggedIn_user_id, QueryParameters queryParameters);

        Task<ResponseResult<List<string>>> GetLatestSearchAsync(int loggedIn_user_id);
    }
}
