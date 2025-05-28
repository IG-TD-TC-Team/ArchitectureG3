using Digitec_WebClient.Models;
using System.Linq;
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
                    UID = Guid.Parse(apiResponse.GetProperty("uid").GetString() ?? Guid.Empty.ToString()),
                    IsStaff = apiResponse.GetProperty("isStaff").GetBoolean()
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
                QuotaCHF = quotaRequest.QuotaCHF,
                Group = quotaRequest.Group ?? ""
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


        public async Task<List<UserM>> creditGroupWithQuotaCHF(string groupName, decimal quotaCHF)
        {
            var url = _baseUrl + "/api/Balance/creditGroupWithQuotaCHF";
            var payload = new
            {
                Group = groupName,
                QuotaCHF = quotaCHF
            };

            var response = await _client.PostAsJsonAsync(url, payload);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Response: {responseContent}");
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                if (apiResponse.TryGetProperty("users", out var usersElement) &&
                    apiResponse.TryGetProperty("quotaCHFCharged", out var quotaChargedElement) &&
                    apiResponse.TryGetProperty("done", out var doneElement) &&
                    doneElement.GetBoolean())
                {
                    var userList = new List<UserM>();


                    foreach (var userElement in usersElement.EnumerateArray())
                    {
                        userList.Add(new UserM
                        {
                            UserID = Guid.Parse(userElement.GetProperty("userID").GetString()),
                            Username = userElement.GetProperty("username").GetString(),
                            Group = userElement.GetProperty("group").GetString(),
                            QuotaCHF = userElement.GetProperty("quotaCHF").GetDecimal(),
                            CopyQuota = userElement.GetProperty("copyQuota").GetInt32()
                        });
                    }

                    return userList;

                }
                else
                {
                    throw new HttpRequestException($"Credit operation failed or unexpected response format: {responseContent}");
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Creditation failed with status code: {response.StatusCode}. Error: {errorContent}");
            }
        }
    }
}