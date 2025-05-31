using MVC_Faculties.Models;
using System.Text.Json;

namespace MVC_Faculties.Services
{
    /// <summary>
    /// Service responsible for handling balance and quota management operations.
    /// This service is focused exclusively on financial transactions and quota adjustments,
    /// following the Single Responsibility Principle by separating these concerns from authentication.
    /// </summary>
    public class BalanceService : IBalanceService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://webapiprintingsystem20250530105714-djb6ebg0d6c4geaq.switzerlandnorth-01.azurewebsites.net/";

        public BalanceService(HttpClient client)
        {
            _client = client;
        }

        /// <summary>
        /// Credits an individual user's printing quota based on their username.
        /// This method handles the HTTP communication with the Balance API endpoint,
        /// processes the response, and returns the updated user information.
        /// </summary>
        /// <param name="quotaRequest">Contains the username, amount to credit, and user's group information</param>
        /// <returns>Updated UserM object with the credited quota information</returns>
        public async Task<UserM> CreditUsernameWithQuotaCHF(UserM quotaRequest)
        {
            var url = _baseUrl + "/api/Balance/creditUsernameWithQuotaCHF";

            // Prepare the payload with user information and quota amount
            var payload = new
            {
                Username = quotaRequest.Username,
                QuotaCHF = quotaRequest.QuotaCHF,
                Group = quotaRequest.Group ?? "" // Ensure group is never null
            };

            try
            {
                // Send the quota credit request to the API
                var response = await _client.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the successful response from the Balance API
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Balance API Response: {responseContent}"); ///Debug

                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Verify that the operation was completed successfully
                    if (apiResponse.TryGetProperty("quotaCHFCharged", out var quotaChargedElement) &&
                        apiResponse.TryGetProperty("done", out var doneElement) &&
                        doneElement.GetBoolean())
                    {
                        // Return the updated user information
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
                        throw new InvalidOperationException($"Credit operation failed or returned unexpected format: {responseContent}");
                    }
                }
                else
                {
                    // Handle API errors with detailed information
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Balance service request failed with status {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Balance service communication error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse balance service response: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Credits all users within a specified group with the same quota amount.
        /// This is a bulk operation that affects multiple users simultaneously,
        /// typically used for semester-wide allocations or department-level quota adjustments.
        /// </summary>
        /// <param name="groupName">The target group name (e.g., "student", "staff", "faculty")</param>
        /// <param name="quotaAmount">The amount of quota to add to each user in the group</param>
        /// <returns>List of all users who received the quota credit</returns>
        public async Task<List<UserM>> CreditGroupWithQuotaCHF(string groupName, decimal quotaAmount)
        {
            var url = _baseUrl + "/api/Balance/creditGroupWithQuotaCHF";

            // Prepare the group operation payload
            var payload = new
            {
                Group = groupName,
                QuotaCHF = quotaAmount
            };

            try
            {
                // Send the group credit request to the API
                var response = await _client.PostAsJsonAsync(url, payload);

                if (response.IsSuccessStatusCode)
                {
                    // Parse the response containing information about all affected users
                    var responseContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Group Balance API Response: {responseContent}"); /// Debug

                    var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                    // Verify the operation was successful and extract user information
                    if (apiResponse.TryGetProperty("users", out var usersElement) &&
                        apiResponse.TryGetProperty("quotaCHFCharged", out var quotaChargedElement) &&
                        apiResponse.TryGetProperty("done", out var doneElement) &&
                        doneElement.GetBoolean())
                    {
                        // Build a list of all users who were affected by the group operation
                        var creditedUsers = new List<UserM>();

                        foreach (var userElement in usersElement.EnumerateArray())
                        {
                            creditedUsers.Add(new UserM
                            {
                                UserID = Guid.Parse(userElement.GetProperty("userID").GetString()),
                                Username = userElement.GetProperty("username").GetString(),
                                Group = userElement.GetProperty("group").GetString(),
                                QuotaCHF = userElement.GetProperty("quotaCHF").GetDecimal(),
                                CopyQuota = userElement.GetProperty("copyQuota").GetInt32()
                            });
                        }

                        return creditedUsers;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Group credit operation failed or returned unexpected format: {responseContent}");
                    }
                }
                else
                {
                    // Handle API errors for group operations
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Group balance service request failed with status {response.StatusCode}: {errorContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException($"Group balance service communication error: {ex.Message}", ex);
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException($"Failed to parse group balance service response: {ex.Message}", ex);
            }
        }
    }
}