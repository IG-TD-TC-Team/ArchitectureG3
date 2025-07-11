﻿using System.Net.Http.Json;
using MVC_POS.Models;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;

namespace MVC_POS.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _client;
        private readonly string _baseUrl = "https://webapiprintingsystem20250530105714-djb6ebg0d6c4geaq.switzerlandnorth-01.azurewebsites.net/api/Authentication/";

        public AuthenticationService(HttpClient client)
        {
            _client = client;
        }

        public async Task<AuthenticationM> AuthenticateByCard(Guid cardID)
        {
            var url = _baseUrl + "authenticateByCard";

            var payload = new
            {
                cardID = cardID
            };

            var response = await _client.PostAsJsonAsync(url, payload);

            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"API Response Status: {response.StatusCode}");
            Console.WriteLine($"API Response Content: {responseContent}");

            if (response.IsSuccessStatusCode)
            {
                var apiResponse = JsonSerializer.Deserialize<JsonElement>(responseContent);

                return new AuthenticationM
                {
                    Message = apiResponse.GetProperty("message").GetString() ?? "",
                    UserID = Guid.Parse(apiResponse.GetProperty("uid").GetString() ?? Guid.Empty.ToString())
                };
            }


            return new AuthenticationM
            {
                Message = $"Authentication failed with status code: {response.StatusCode}",
                UserID = Guid.Empty
            };
        }
    }
 }
