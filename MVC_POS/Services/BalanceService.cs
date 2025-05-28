using MVC_POS.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace MVC_POS.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7101api/Balance/";

        public BalanceService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserM> CreditUIDWithQuotaCHF(UserM quotaRequest)
        {
            var url = _baseUrl + "/creditUIDWithQuotaCHF";

            var payload = new
            {
                UserID = quotaRequest.UserID,
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
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"API call failed with status code {response.StatusCode}: {errorContent}");
            }
        }
    }
}
