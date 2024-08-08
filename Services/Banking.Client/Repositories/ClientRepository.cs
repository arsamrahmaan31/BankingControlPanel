using Banking.Client.Constants;
using Banking.Client.Models;
using Dapper;
using Newtonsoft.Json;
using Serilog;
using System.Data;
using System.Data.SqlClient;
using System.Net;

namespace Banking.Client.Repositories
{
    public class ClientRepository(IConfiguration configuration) : IClientRepository
    {
        #region Public methods
        public async Task<ResponseResult<AddClientResponse>> CreateClientAsync(AddClientRequest client, string? filePath)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                // Begin a database transaction
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Add the new client to the database
                        var client_id = await AddNewClientAsync(client,filePath, connection, transaction);

                        // Check if the client was added successfully
                        if (client_id.HasValue && client_id.Value > 0)
                        {
                            // Add client accounts associated with the new client
                            await AddClientAccountsAsync(client, client_id.Value, connection, transaction);

                            // Commit the transaction
                            transaction.Commit();

                            // Return success response
                            return new ResponseResult<AddClientResponse> { success = true, status_code = (int)HttpStatusCode.OK, result = null, message = StaticMessages.ClientAdded };
                        }
                        else
                        {
                            // Rollback the transaction if client insertion failed
                            transaction.Rollback();
                            return new ResponseResult<AddClientResponse> { success = false, status_code= (int)HttpStatusCode.InternalServerError, result = null, message = StaticMessages.ClientInsertionFailed };
                        }
                    }
                    catch (Exception ex)
                    {
                        // Rollback the transaction in case of an exception
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(CreateClientAsync), ex.Message);
                throw;
            }
        }

        public async Task<bool> CheckIfValidAdmin(int added_by_id)
        {
            try
            {
                await using var connection = CreateConnection();
                await connection.OpenAsync();

                // Passing added_by_id as a dynamic parameter for the stored procedure
                var parameters = new { added_by_id };

                // Execute the stored procedure
                bool isValid = await connection.QueryFirstOrDefaultAsync<bool>(
                    SystemConstants.StoredProcedure_CheckIfValidAdmin, parameters, commandType: CommandType.StoredProcedure
                );

                // Return the result
                return isValid;
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(CheckIfValidAdmin), sqlEx);
                throw;
            }
            catch (Exception ex)
            {
                // Log any other exceptions
                Log.Error(StaticMessages.ExceptionOccured, nameof(CheckIfValidAdmin), ex);
                throw;
            }
        }


        public async Task<IEnumerable<Clients>> GetAllClientsAsync(int loggedIn_user_id, QueryParameters queryParameters)
        {
            try
            {
                // Log the filter data if filter is not null
                if (!string.IsNullOrEmpty(queryParameters.filter))
                {
                    await LogSearchFilterAsync(loggedIn_user_id,queryParameters.filter);
                }

                using var connection = CreateConnection();
                await connection.OpenAsync();

                // prepare the parameters
                var parameters = new
                {
                    Filter = queryParameters.filter,
                    SortBy = queryParameters.sortBy,
                    SortDescending = queryParameters.sortDescending,
                    PageNumber = queryParameters.pageNumber,
                    PageSize = queryParameters.pageSize
                };

                // Pass the params to the stored procedure to fetch the data
                var clientDtos = await connection.QueryAsync<ClientDto>( SystemConstants.StoreProcedure_GetAllClients,parameters,commandType: CommandType.StoredProcedure );


                // NOTE: Using AutoMapper would be a cleaner and more scalable solution for mapping query parameters.
                // However, since this is a single instance and for simplicity, we are manually mapping the parameters here.
                var clients = clientDtos.Select(dto => MapToClients(dto));
                return clients;
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetAllClientsAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetAllClientsAsync), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetSuggesstionsAsync(int loggedIn_user_id)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                // Create dynamic param
                var parameters = new { loggedIn_user_id };

                // Fetch list using the stored procedure
                var searchFilters = await connection.QueryAsync<string>(
                    SystemConstants.StoreProcedure_GetLast3SearchSuggestions,
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return searchFilters;
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetSuggesstionsAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetSuggesstionsAsync), ex.Message);
                throw; 
            }
        }

        #endregion


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

        private Clients MapToClients(ClientDto dto)
        {
            // Mapping the DTO with the client model
            return new Clients
            {
                gender_name = dto.gender_name,
                first_name = dto.first_name,
                last_name = dto.last_name,
                email = dto.email,
                personal_id = dto.personal_id,
                profile_photo = dto.profile_photo,
                mobile_number = dto.mobile_number,
                client_accounts = !string.IsNullOrEmpty(dto.client_accounts)
                    ? JsonConvert.DeserializeObject<List<ClientAccount>>(dto.client_accounts)
                    : new List<ClientAccount>(),
                address = !string.IsNullOrEmpty(dto.address)
                    ? JsonConvert.DeserializeObject<ClientAddress>(dto.address)
                    : new ClientAddress()
            };
        }

        private static async Task<int?> AddNewClientAsync(AddClientRequest clientRequest,string? filePath, SqlConnection connection,SqlTransaction transaction)
        {
            try
            {
                // Create DynamicParameters object to hold the parameters for the stored procedure
                var parameters = new DynamicParameters();

                // Add parameters for client details
                parameters.Add(DatabaseClientFields.Personal_Id, clientRequest.personal_id);
                parameters.Add(DatabaseClientFields.AddedBy, clientRequest.added_by_id);
                parameters.Add(DatabaseClientFields.Gender_Id, clientRequest.gender_id);
                parameters.Add(DatabaseClientFields.Email_Add, clientRequest.email);
                parameters.Add(DatabaseClientFields.ProfilePhoto, filePath);
                parameters.Add(DatabaseClientFields.First_Name, clientRequest.first_name);
                parameters.Add(DatabaseClientFields.Last_Name, clientRequest.last_name);
                parameters.Add(DatabaseClientFields.Mobile_Number, clientRequest.mobile_number);
                parameters.Add(DatabaseClientFields.Zip_Code, clientRequest.address.zip_code);
                parameters.Add(DatabaseClientFields.City_, clientRequest.address.city);
                parameters.Add(DatabaseClientFields.Street_, clientRequest.address.street);
                parameters.Add(DatabaseClientFields.Country_, clientRequest.address.country);

                // Add an output parameter to retrieve the generated client ID
                parameters.Add(DatabaseClientFields.Client_Id, dbType: DbType.Int32, direction: ParameterDirection.Output);

                // Execute the stored procedure to add the new client
                await connection.ExecuteAsync(SystemConstants.StoreProcedure_AddNewClient, parameters, transaction, commandType: CommandType.StoredProcedure);

                // Retrieve the output parameter value (Client ID) and return it
                int? clientId = parameters.Get<int?>(DatabaseClientFields.Client_Id);
                return clientId;
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddNewClientAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddNewClientAsync), ex.Message);
                throw;
            }

        }

        private static async Task AddClientAccountsAsync(AddClientRequest clientRequest, int client_id, SqlConnection connection, SqlTransaction transaction)
        {
            try
            {
                // Check if there are any client accounts to add
                if (clientRequest.client_accounts != null)
                {
                    // Iterate over each account in the client accounts list
                    foreach (var account in clientRequest.client_accounts)
                    {
                        // Prepare parameters for the stored procedure
                        var account_number_Parameters = new { account.account_number, client_id };

                        // Execute the stored procedure to add the client account
                        await connection.QueryFirstOrDefaultAsync<AddClientResponse>( SystemConstants.StoreProcedure_AddClientAccounts, account_number_Parameters, transaction, commandType: CommandType.StoredProcedure);
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddClientAccountsAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddClientAccountsAsync), ex.Message);
                throw;
            }
        }

        private async Task LogSearchFilterAsync(int loggedIn_user_id, string filter)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                // Prepare parameters for the stored procedure
                var parameters = new { filter, loggedIn_user_id, searched_at = DateTime.Now };

                // Execute the stored procedure to store the searched filter
                await connection.ExecuteAsync(SystemConstants.StoredProcedure_LogSearchFilters, parameters, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(LogSearchFilterAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(LogSearchFilterAsync), ex.Message);
                throw;
            }
        }

        #endregion
    }
}
