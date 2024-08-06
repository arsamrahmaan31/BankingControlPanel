using Banking.Client.Constants;
using Banking.Client.Models;
using Serilog;

namespace Banking.Client.Managers
{
    public class ClientManager(IClientRepository clientRepository) : IClientManager
    {
        public async Task<ResponseResult<AddClientResponse>> AddClientAsync(AddClientRequest client)
        {
            try
            {
                // Add client in the database
                ResponseResult<AddClientResponse> createUserResult = await clientRepository.SignUpAsync(client);

                if (createUserResult.success)
                {
                    return new ResponseResult<AddClientResponse>
                    {
                        success = true,
                        result = createUserResult.result,
                        message = StaticMessages.ClientAdded
                    };
                }
                else
                {
                    return new ResponseResult<AddClientResponse>
                    {
                        success = false,
                        result = null,
                        message = StaticMessages.SomethingWentWrong
                    };
                }
            }
            catch (Exception ex)
            {
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddClientAsync), ex.Message);
                return new ResponseResult<AddClientResponse>
                {
                    success = false,
                    result = null,
                    message = ex.Message
                };
            }
        }
    }
}
