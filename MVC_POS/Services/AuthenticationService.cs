using System.Net.Http.Json;
using MVC_POS.Models;
using System;
using System.Threading.Tasks;
using System.Text.Json;

namespace MVC_POS.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7101/api/Authentication/";

        public AuthenticationService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AuthenticationM> AuthenticateByCard(Guid cardID)
        {
            var url = _baseUrl + "authenticateByCard";

            var response = await _client.PostAsJsonAsync(url, cardID);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new AuthenticationM
                {
                    Message = apiResponse.GetProperty("message").GetString() ?? "",
                    UserID = apiResponse.GetProperty("UID").GetGuid()
                };
            }

            return new AuthenticationM
            {
                Message = $"Authentication failed with status code: {response.StatusCode}",
                UserID = Guid.Empty
            };
        }
    }
 }
