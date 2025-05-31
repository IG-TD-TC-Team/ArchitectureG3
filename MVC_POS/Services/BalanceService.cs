using MVC_POS.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace MVC_POS.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://webapiprintingsystem20250530105714-djb6ebg0d6c4geaq.switzerlandnorth-01.azurewebsites.net/api/Balance";

        public BalanceService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserM> CreditUIDWithQuotaCHF(UserM quotaRequest)
        {
            var url = $"{_baseUrl}/creditUIDWithQuotaCHF"; 

            var payload = new
            {
                userID = quotaRequest.UserID,  
                quotaCHF = quotaRequest.QuotaCHF
            };

            var response = await _client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                
                if (apiResponse.TryGetProperty("newQuotaCHF", out var quotaElement) &&
                    apiResponse.TryGetProperty("newPrintQuota", out var printQuotaElement) &&
                    apiResponse.TryGetProperty("done", out var doneElement) &&
                    doneElement.GetBoolean())
                {
                    return new UserM
                    {
                        UserID = quotaRequest.UserID,
                        QuotaCHF = quotaElement.GetDecimal(),
                        CopyQuota = printQuotaElement.GetInt32()
                    };
                }
                else
                {
                    throw new HttpRequestException($"Credit operation failed or unexpected response format: {responseContent}");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
            }
        }
    }
}