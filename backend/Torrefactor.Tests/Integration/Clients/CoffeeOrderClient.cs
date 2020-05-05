using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Torrefactor.Tests.Integration.Clients
{
    public class CoffeeOrderClient
    {
        private static readonly string _baseUrl = "/api/coffee-orders";
        private readonly HttpClient _client;

        public CoffeeOrderClient(HttpClient client)
        {
            _client = client;
        }

        private static StringContent EmptyContent
            => CreateContent(new { });

        public async Task<ApiResponse> AddPackToOrder(string coffeeName, int weight)
        {
            return await _client.PostAsync(GetAddPackUrl(coffeeName, weight), EmptyContent).ToApiResponse();
        }

        public async Task<ApiResponse> RemovePackFromOrder(string coffeeName, int weight)
        {
            return await _client.DeleteAsync(GetRemovePackUrl(coffeeName, weight)).ToApiResponse();
        }

        public async Task<ApiResponse> CreateNewGroupOrder()
        {
            return await _client.PostAsync(GetCreateNewOrderUrl(), EmptyContent).ToApiResponse();
        }

        public async Task<ApiResponse> SendToCoffeeProvider()
        {
            return await _client.PostAsync(GetSendToCoffeeProviderUrl(), EmptyContent).ToApiResponse();
        }

        private static StringContent CreateContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }


        private static string GetAddPackUrl(string coffee, int weight)
        {
            return $"{_baseUrl}/current-user/{coffee}/{weight}";
        }

        private string GetRemovePackUrl(string coffee, int weight)
        {
            return $"{_baseUrl}/current-user/{coffee}/{weight}";
        }

        private string GetSendToCoffeeProviderUrl()
        {
            return $"{_baseUrl}/send";
        }


        private static string GetCreateNewOrderUrl()
        {
            return $"{_baseUrl}";
        }
    }
}