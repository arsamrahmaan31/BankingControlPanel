using Banking.Client.Constants;
using Banking.Client.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace Banking.Client.Repositories
{
    public class ClientRepository(IConfiguration configuration) : IClientRepository
    {
        public async Task<ResponseResult<AddClientResponse>> CreateClientAsync(AddClientRequest client)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        var client_id = await AddNewClientAsync(client, connection, transaction);

                        if (client_id.HasValue && client_id.Value > 0)
                        {
                            await AddClientAccountsAsync(client, client_id.Value, connection, transaction);
                            transaction.Commit();
                            return new ResponseResult<AddClientResponse>
                            {
                                success = true,
                                status_code = (int)HttpStatusCode.OK,
                                result = null,
                                message = StaticMessages.ClientAdded,
                            };
                        }
                        else
                        {
                            transaction.Rollback();
                            return new ResponseResult<AddClientResponse> { success = false, status_code= (int)HttpStatusCode.InternalServerError, result = null, message = StaticMessages.ClientInsertionFailed };
                        }
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(StaticMessages.ExceptionOccured, nameof(CreateClientAsync), ex.Message);
                throw;
            }
        }


        #region Private methods

        private SqlConnection CreateConnection()
        {
            var connectionString = configuration.GetConnectionString(SystemConstants.DefaultConnection);
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(StaticMessages.DatabaseNotConfigured);
            }
            return new SqlConnection(connectionString);
        }


        private static async Task<int?> AddNewClientAsync(
    AddClientRequest clientRequest,
    SqlConnection connection,
    SqlTransaction transaction)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@personal_id", clientRequest.personal_id);
            parameters.Add("@gender_id", clientRequest.gender_id);
            parameters.Add("@profile_photo", clientRequest.profile_photo);
            parameters.Add("@email", clientRequest.email);
            parameters.Add("@first_name", clientRequest.first_name);
            parameters.Add("@last_name", clientRequest.last_name);
            parameters.Add("@mobile_number", clientRequest.mobile_number);
            parameters.Add("@zip_code", clientRequest.address.zip_code);
            parameters.Add("@city", clientRequest.address.city);
            parameters.Add("@street", clientRequest.address.street);
            parameters.Add("@country", clientRequest.address.country);

            // Add output parameter
            parameters.Add("@client_id", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync(
                SystemConstants.StoreProcedure_AddNewClient,
                parameters,
                transaction,
                commandType: CommandType.StoredProcedure
            );

            // Retrieve output parameter value
            int? clientId = parameters.Get<int?>("@client_id");
            return clientId;
        }


        private static async Task AddClientAccountsAsync(AddClientRequest clientRequest, int client_id, SqlConnection connection, SqlTransaction transaction)
        {
            if (clientRequest.client_accounts != null)
            {
                foreach (var account in clientRequest.client_accounts)
                {
                    var account_number_Parameters = new
                    {
                        account.account_number,
                        client_id,
                    };

                    await connection.QueryFirstOrDefaultAsync<AddClientResponse>(
                        SystemConstants.StoreProcedure_AddClientAccounts,
                        account_number_Parameters,
                        transaction,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
        }

        #endregion

    }
}
