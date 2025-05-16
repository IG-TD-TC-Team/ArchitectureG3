namespace MVC_POS.Services
{
    public class BalanceService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = ""; //TODO: Add the base URL of the API
        public BalanceService(HttpClient client)
        {
            _client = client;
        }

        public async Task<UserM> CreditUserWithQuotaCHFAsync(Guid userId, decimal quotaCHF)
        {
            var url = _baseUrl + "/BalanceController";
            var payload = new { UserID = userId, QuotaCHF = quotaCHF };
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
