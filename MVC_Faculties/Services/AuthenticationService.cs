using MVC_Faculties.Models;
using System.Text.Json;

namespace MVC_Faculties.Services
{
    /// <summary>
    /// Service responsible for handling user authentication operations.
    /// This service focuses solely on authentication concerns, separated from balance management.
    /// </summary>
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://webapiprintingsystem20250530105714-djb6ebg0d6c4geaq.switzerlandnorth-01.azurewebsites.net/";

        public AuthenticationService(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Authenticates a user using their username and password.
        /// This method communicates with the WebAPI Authentication controller to verify credentials
        /// and determine if the user has staff privileges for group operations.
        /// </summary>
        /// <param name="userAuth">Contains the username and password for authentication</param>
        /// <returns>Authentication result including success status, user ID, and staff permissions</returns>
        public async Task<AuthentificationM> AuthenticateByUsername(AuthentificationM userAuth)
        {
            var url = _baseUrl + "/api/Authentication/authenticateByUsername";

            // Prepare the authentication payload
            var payload = new
            {
                Username = userAuth.Username,
                Password = userAuth.Password
            };

            try
            {
                // Send authentication request to the API
                var response = await _client.PostAsJsonAsync(url, payload);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse the successful authentication response
                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    return new AuthentificationM
                    {
                        Username = userAuth.Username,
                        Password = userAuth.Password, 
                        Message = apiResponse.GetProperty("message").GetString() ?? "",
                        UID = Guid.Parse(apiResponse.GetProperty("uid").GetString() ?? Guid.Empty.ToString()),
                        IsStaff = apiResponse.GetProperty("isStaff").GetBoolean(),
                        Group = apiResponse.GetProperty("group").GetString() ?? "unknown"
                    };
                }
                else
                {
                    // Handle authentication failure scenarios
                    var errorResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
                    var errorMessage = errorResponse.GetProperty("message").GetString() ?? "Authentication failed";

                    return new AuthentificationM
                    {
                        Username = userAuth.Username,
                        Message = errorMessage,
                        UID = Guid.Empty,
                        IsStaff = false,
                        Group = "unknown"
                    };
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Authentication service communication error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse authentication response: {ex.Message}", ex);
            }
        }
    }
}