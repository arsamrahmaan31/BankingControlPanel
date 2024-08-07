using Banking.Client.Models;

namespace Banking.Client.Repositories
{
    public interface IClientRepository
    {
        Task<ResponseResult<AddClientResponse>> CreateClientAsync(AddClientRequest client, string? filePath);
        Task<IEnumerable<Clients>> GetAllClientsAsync(int loggedIn_user_id, QueryParameters queryParameters);
        Task<IEnumerable<string>> GetSuggesstionsAsync(int loggedIn_user_id);
    }
}
