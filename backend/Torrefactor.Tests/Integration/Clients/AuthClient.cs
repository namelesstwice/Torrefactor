using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;

namespace Torrefactor.Tests.Integration.Clients
{
    public static class AuthExtensions
    {
        public static AuthClient CreateAuthClient(this TestServer testServer) => new AuthClient(testServer.CreateClient());
    }

    public class AuthClient
    {
        public AuthClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse> SignIn(string email, string password)
        {
            var request = new
            {
                email,
                password
            };
            var response = await _client.PostAsync(_signInUrl, CreateContent(request)).ToApiResponse();
            
            if (response.Json.Value<bool>("success"))
            {
                var token = response.Json.Value<string>("accessToken");
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            return response;
        }

        public async Task<ApiResponse> Register(string email, string password, string name)
        {
            var regRequest = new
            {
                name,
                password,
                email
            };
            return await _client.PostAsync(_registerUrl, CreateContent(regRequest)).ToApiResponse();
        }

        public async Task<ApiResponse> GetCurrentUser()
        {
            return await _client.GetAsync(_userUrl).ToApiResponse();
        }

        public async Task<ApiResponse> ConfirmUser(string id)
        {
           return await _client.PutAsync(GetUserConfirmUrl(id, true), EmptyContent)
               .ToApiResponse();
        }        
        
        public async Task<ApiResponse> GetNotConfirmedUsers()
        {
           return await _client.GetAsync(_notConfirmedUrl)
               .ToApiResponse();
        }

        private static StringContent EmptyContent
            => CreateContent(new { });

        private static StringContent CreateContent(object obj) 
            => new StringContent(JsonConvert.SerializeObject(obj), encoding: Encoding.UTF8, "application/json");

        private static string GetUserConfirmUrl(string id, bool isApproved) 
            => $"{_baseUrl}/users/{id}/confirmation-state?isApproved={isApproved}";

        private static readonly string _baseUrl = "/api/auth";
        private static readonly string _signInUrl = $"{_baseUrl}/sign-in";
        private static readonly string _userUrl = $"{_baseUrl}/users/current";
        private static readonly string _notConfirmedUrl = $"{_baseUrl}/users/not-confirmed";
        private static readonly string _registerUrl = $"{_baseUrl}/register";
        
        private readonly HttpClient _client;
    }
}