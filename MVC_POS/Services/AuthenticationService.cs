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
        private readonly string _baseUrl = ""; //TODO: Add the base URL of the API

        public AuthenticationService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AuthenticationM> AuthenticateByCardAsync(Guid cardId)
        {
            var url = _baseUrl + "/api/Authentication/authenticateByCard";
            var payload = cardId;
            var json = JsonSerializer.Serialize(payload);
            var response = await _client.PostAsJsonAsync(url, json);
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthenticationM>(responseContent);
            }
            throw new HttpRequestException($"Authentication failed with status code: {response.StatusCode}");
        }
    }
}
