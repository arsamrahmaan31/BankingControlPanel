using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Banking.Auth.Models;
using Dapper;
using Serilog;
using Banking.Auth.Constants;

namespace Banking.Auth.Repositories
{
    public class AuthRepository(IConfiguration configuration): IAuthRepository
    {

        public async Task<UserVerificationResult> IsLoginExistsAsync(string email)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                // Passing email as a dynamic parameter for the stored procedure
                var parameters = new { email };

                // Execute the stored procedure
                var user = await connection.QueryFirstOrDefaultAsync<UserVerificationResult>(
                    SystemConstants.StoredProcedure_CheckIfEmailExists, parameters, commandType: CommandType.StoredProcedure
                );

                // Return the result
                return user;
            }
            catch (SqlException sqlEx)
            {
                // Log the SQL-specific exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), sqlEx.Message);
                throw sqlEx.InnerException;
            }
            catch (Exception ex)
            {
                // Log the application-specific exceptions
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), ex.Message);
                throw ex;
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

        #endregion

    }
}
