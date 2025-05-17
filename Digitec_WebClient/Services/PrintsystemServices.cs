using Digitec_WebClient.Models;
using System.Text.Json;

namespace MVC_Faculties.Services
{
    public class PrintsystemServices : IPrintsystemServices
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7101"; //TODO: Add the base URL of the API

            public PrintsystemServices(HttpClient client)
            {
                _client = client;
            }

        public async Task<AuthentificationM> AuthenticateByUsername(AuthentificationM userAuth)
        {
            var url = _baseUrl + "/AuthenticationController";
            var payload = userAuth;
            

            var json = JsonSerializer.Serialize(payload);
            
            var response = await _client.PostAsJsonAsync(url,json);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<AuthentificationM>(responseContent);
            }

            throw new HttpRequestException($"Authentication failed with status code: {response.StatusCode}");
        }

        public async Task<UserM> creditUsernameWithQuotaCHF(UserM user)
        {
            var url = _baseUrl + "/BalanceController";

            var payload = user;
          
            var json = JsonSerializer.Serialize(payload);
            var response = await _client.PostAsJsonAsync(url, json);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                
                return JsonSerializer.Deserialize<UserM>(responseContent);
            }

            throw new HttpRequestException($"Creditation failed with status code: {response.StatusCode}");

        }
    }
}
