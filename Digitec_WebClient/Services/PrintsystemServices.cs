using Digitec_WebClient.Models;
using System.Net;
using System.Text.Json;

namespace MVC_Faculties.Services
{
    public class PrintsystemServices : IPrintsystemServices
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7101"; 

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
            var url = _baseUrl + "/api/Balance/creditUsernameWithQuotaCHF";

            
            var payload = new
            {
                Username = quotaRequest.Username,
                QuotaCHF = quotaRequest.QuotaCHF
            };

            
            var response = await _client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                
                if (apiResponse.TryGetProperty("quotaCHFCharged", out var quotaChargedElement) &&
                    apiResponse.TryGetProperty("done", out var doneElement) &&
                    doneElement.GetBoolean())
                {
                    return new UserM
                    {
                        Username = quotaRequest.Username,
                        Group = quotaRequest.Group, 
                        UserID = quotaRequest.UserID,
                        QuotaCHF = quotaChargedElement.GetDecimal(), 
                        CopyQuota = 0 
                    };
                }
                else
                {
                    throw new HttpRequestException($"Credit operation failed or unexpected response format: {responseContent}");
                }
            }

            
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Creditation failed with status code: {response.StatusCode}. Error: {errorContent}");
        }

        public async Task<List<UserM>> creditGroupWithQuotaCHF(string groupName)
        {
            var url = _baseUrl + "/api/Balance/creditGroupWithQuotaCHF";
            var payload = new
            {
                Group = groupName

            };
            var response = await _client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);
            }

            if (apiResponse.TryGetProperty("quotaCHFCharged", out var quotaChargedElement) &&
                apiResponse.TryGetProperty("done", out var doneElement) &&
                doneElement.GetBoolean())
            {
                return new List<UserM>
                {
                    new UserM
                    {
                        Group = groupName,
                        QuotaCHF = quotaChargedElement.GetDecimal(),
                        CopyQuota = 0
                    }
                };
            }
            else
            {
                throw new HttpRequestException($"Credit operation failed or unexpected response format: {responseContent}");


            }

    }
}