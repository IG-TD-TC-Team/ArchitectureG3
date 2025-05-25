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
            var url = _baseUrl + "/api/Authentication/authenticateByUsername";

            var payload = new
            {
                Username = userAuth.Username,
                Password = userAuth.Password
            };
          
            var response = await _client.PostAsJsonAsync(url, payload);

            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new AuthentificationM
                {
                    Username = userAuth.Username,
                    Password = userAuth.Password,
                    Message = apiResponse.GetProperty("message").GetString() ?? "",
                    UID = Guid.Parse(apiResponse.GetProperty("uid").GetString() ?? Guid.Empty.ToString())
                };
            }

            throw new HttpRequestException($"Authentication failed with status code: {response.StatusCode}");
        }

        public async Task<UserM> creditUsernameWithQuotaCHF(UserM quotaRequest)
        {
            var url = _baseUrl + "/api/Balance/creditUsernameWithQuotaCHF?username={user.Username}&quotaCHF={user.QuotaCHF}";

           var payload = new UserM
            {
                Username = quotaRequest.Username,
                QuotaCHF = quotaRequest.QuotaCHF
                
            };

            var response = await _client.PostAsJsonAsync(url, payload);


           
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new UserM
                {
                    Username = quotaRequest.Username,
                    QuotaCHF = apiResponse.GetProperty("newQuotaCHF").GetDecimal(),
                    CopyQuota = apiResponse.GetProperty("newPrintQuota").GetInt32()
                };

            }

            throw new HttpRequestException($"Creditation failed with status code: {response.StatusCode}");

        }
    }
}
