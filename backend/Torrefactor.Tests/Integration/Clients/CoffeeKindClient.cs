using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Torrefactor.Tests.Integration.Clients
{
    public class CoffeeKindClient
    {
        private readonly string _baseUrl = "/api/coffee-kinds";
        private readonly HttpClient _client;

        public CoffeeKindClient(HttpClient client)
        {
            _client = client;
        }

        private static StringContent EmptyContent
            => CreateContent(new { });

        public async Task<ApiResponse> ReloadCoffeeKinds()
        {
            return await _client.PostAsync(GetReloadCoffeeKindsUrl(), EmptyContent).ToApiResponse();
        }

        public async Task<ApiResponse> GetKinds()
        {
            return await _client.GetAsync(GetKindsUrl()).ToApiResponse();
        }

        private string GetReloadCoffeeKindsUrl()
        {
            return $"{_baseUrl}/reload";
        }

        private string GetKindsUrl()
        {
            return _baseUrl;
        }


        private static StringContent CreateContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }
    }
}