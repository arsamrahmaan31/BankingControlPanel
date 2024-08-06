using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Banking.Auth.Models;
using Dapper;
using Serilog;
using Banking.Auth.Constants;
using System.Net;

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

        public async Task<ResponseResult<SignUpResponse>> SignUpAsync(SignUpRequest user)
        {
            try
            {
                using var connection = CreateConnection();
                await connection.OpenAsync();

                // Parameters for the stored procedure
                var parameters = new { user.role_id, user.first_name, user.last_name, user.email, user.password };

                // Execute the stored procedure
                var userRes = await connection.QueryFirstOrDefaultAsync<SignUpResponse>(
                    SystemConstants.StoredProcedure_UserSignUp, parameters, commandType: CommandType.StoredProcedure
                );

                // Return success response
                return new ResponseResult<SignUpResponse>
                {
                    success = true, status_code=(int)HttpStatusCode.OK, result = userRes, message = StaticMessages.UserCreated
                };
            }
            catch (SqlException sqlEx)
            {
                // Handle SQL-specific exceptions
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), sqlEx.Message);
                return new ResponseResult<SignUpResponse>
                {
                    success = false,
                    status_code = (int)HttpStatusCode.InternalServerError,
                     result = null, message = StaticMessages.DatabaseErrorOccured
                };
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Log.Error(StaticMessages.ExceptionOccured, nameof(SignUpAsync), ex.Message);
                return new ResponseResult<SignUpResponse>
                {
                    success = false,
                    status_code = (int)HttpStatusCode.InternalServerError,
                    result = null, message = ex.Message
                };
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
