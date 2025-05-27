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
        private readonly string _baseUrl;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthenticationService(HttpClient client)
        {
            _client = client;
            _baseUrl = "https://localhost:7101"; 
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<AuthenticationM> AuthenticateByCardAsync(Guid cardId)
        {
            try
            {
                // API endpoint for card authentication
                string endpoint = "/api/Authentication/authenticateByCard";
                string url = $"{_baseUrl}{endpoint}";

                // Send the card ID directly as the request body
                var response = await _client.PostAsJsonAsync(url, cardId);

                // Read the response content regardless of status code
                var responseContent = await response.Content.ReadAsStringAsync();

                // Log the response for debugging if needed
                Console.WriteLine($"API Response: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        // Try to deserialize the successful response
                        var apiResponse = JsonSerializer.Deserialize<ApiAuthResponse>(responseContent, _jsonOptions);

                        return new AuthenticationM
                        {
                            Message = apiResponse.Message,
                            UID = apiResponse.UID,
                        };
                    }
                    catch (JsonException ex)
                    {
                        Console.WriteLine($"JSON deserialization error: {ex.Message}");
                        return new AuthenticationM
                        {
                            Message = $"Error processing authentication response: {ex.Message}",
                            UID = null
                        };
                    }
                }
                else
                {
                    // Handle error response
                    try
                    {
                        var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(responseContent, _jsonOptions);
                        return new AuthenticationM
                        {
                            Message = errorResponse.Message ?? $"Authentication failed with status code {response.StatusCode}",
                            UID = null
                        };
                    }
                    catch
                    {
                        // If we can't deserialize the error, return a generic message
                        return new AuthenticationM
                        {
                            Message = $"Authentication failed with status code {response.StatusCode}",
                            UID = null
                        };
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                // Handle network or connection errors
                return new AuthenticationM
                {
                    Message = $"Connection error: {ex.Message}",
                    UID = null
                };
            }
            catch (Exception ex)
            {
                // Handle any other unexpected errors
                return new AuthenticationM
                {
                    Message = $"Unexpected error: {ex.Message}",
                    UID = null
                };
            }
        }
    }
 }
