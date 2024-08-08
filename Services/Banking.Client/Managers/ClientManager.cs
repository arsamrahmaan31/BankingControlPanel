using Banking.Client.Constants;
using Banking.Client.Models;
using Banking.Client.Repositories;
using Serilog;
using System.Net;

namespace Banking.Client.Managers
{
    public class ClientManager(IClientRepository clientRepository) : IClientManager
    {
        public async Task<ResponseResult<AddClientResponse>> AddClientAsync(AddClientRequest client)
        {
            try
            {
                // Check if the user trying to add a client is a valid admin
                bool isValid = await clientRepository.CheckIfValidAdmin(client.added_by_id);
                if (!isValid)
                {
                    return new ResponseResult<AddClientResponse> { success = false, status_code = (int)HttpStatusCode.Forbidden, result = null, message = StaticMessages.NotValidAdmin };
                }
                string filePath = null;

                // Check if a profile picture is included in the request
                if (client.profile_picture != null && client.profile_picture.Length > 0)
                {
                    // Define the directory path where uploaded pictures will be stored
                    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedPictures");

                    // Ensure the directory exists; create it if it does not
                    if (!Directory.Exists(uploadsFolderPath))
                    {
                        Directory.CreateDirectory(uploadsFolderPath);
                    }

                    // Generate a unique file name using GUID and the original file extension
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(client.profile_picture.FileName);
                    filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                    // Save the profile picture to the specified path
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await client.profile_picture.CopyToAsync(stream);
                    }

                    // Set the relative path of the uploaded profile picture for storage in the database
                    filePath = $"/UploadedPictures/{uniqueFileName}";
                }

                // Add client in the database
                ResponseResult<AddClientResponse> createUserResult = await clientRepository.CreateClientAsync(client, filePath);

                // If insertion is successful, return a success response with a 200 OK status
                if (createUserResult.success)
                {
                    return new ResponseResult<AddClientResponse> { success = true, status_code = (int)HttpStatusCode.OK, result = createUserResult.result, message = StaticMessages.ClientAdded };
                }

                // If insertion fails, return an error response with a 500 Internal Server Error status
                else
                {
                    return new ResponseResult<AddClientResponse> { success = false, status_code= (int)HttpStatusCode.InternalServerError, result = null, message = StaticMessages.SomethingWentWrong };
                }
            }
            catch (Exception ex)
            {
                //Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(AddClientAsync), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }


        public async Task<ResponseResult<List<Clients>>> GetClientsAsync(int loggedIn_user_id, QueryParameters queryParameters)
        {
            try
            {
                // Retrieve data using the repository method
                var clients = await clientRepository.GetAllClientsAsync(loggedIn_user_id, queryParameters);

                // Convert the IEnumerable<Clients> to a List<Clients>
                var clientList = clients.ToList();

                // Return client list if found
                if (clientList.Count > 0)
                {
                    return new ResponseResult<List<Clients>> { success = true, status_code = (int)HttpStatusCode.OK, result = clientList, message = StaticMessages.ClientsFound };
                }

                // Else no client found
                return new ResponseResult<List<Clients>> { success = false, status_code = (int)HttpStatusCode.NotFound, result = new List<Clients>(), message = StaticMessages.NoClientsFound };
            }
            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetClientsAsync), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }

        public async Task<ResponseResult<List<string>>> GetLatestSearchAsync(int loggedIn_user_id)
        {
            try
            {
                // Retrieve latest suggesstions from database
                var searchFilters = await clientRepository.GetSuggesstionsAsync(loggedIn_user_id);

                // Convert the IEnumerable<string> to a List<string>
                var searchFilterList = searchFilters.ToList();

                // Check if any search filters were found, return the list
                if (searchFilterList.Count > 0)
                {
                    return new ResponseResult<List<string>> { success = true, status_code = (int)HttpStatusCode.OK, result = searchFilterList, message = StaticMessages.SuggesstionsFound };
                }

                // No record found
                return new ResponseResult<List<string>> { success = false, status_code = (int)HttpStatusCode.NotFound, result = new List<string>(), message = StaticMessages.NoSuggesstionsFound };
            }

            catch (Exception ex)
            {
                // Log the exception
                Log.Error(StaticMessages.ExceptionOccured, nameof(GetLatestSearchAsync), ex.Message);

                // The exception will be handled by the global exception middleware
                throw;
            }
        }

    }
}
