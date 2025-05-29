using MVC_POS.Models;
using System.Text.Json;
using System.Net.Http.Json;

namespace MVC_POS.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://localhost:7101/api/Balance"; // ✅ Fixed: added missing slash after port

        public BalanceService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserM> CreditUIDWithQuotaCHF(UserM quotaRequest)
        {
            var url = $"{_baseUrl}/creditUIDWithQuotaCHF"; // ✅ Clean URL construction

            var payload = new
            {
                userID = quotaRequest.UserID,  // ✅ Match the API parameter name (lowercase 'u')
                quotaCHF = quotaRequest.QuotaCHF
            };

            var response = await _client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                // ✅ Match the actual API response property names (from your curl result)
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