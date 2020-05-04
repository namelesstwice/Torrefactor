using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Torrefactor.Tests.Integration.Clients
{
    public class CoffeeOrderClient
    {
        private readonly HttpClient _client;
        private static readonly string _baseUrl = "/api/coffee-orders";
        
        public CoffeeOrderClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<ApiResponse> AddPackToOrder(string coffeeName, int weight)
        {
            return await _client.PostAsync(GetAddPackUrl(coffeeName, weight), EmptyContent).ToApiResponse();
        }

        public async Task<ApiResponse> CreateNewGroupOrder()
        {
            return await _client.PostAsync(GetCreateNewOrderUrl(), EmptyContent).ToApiResponse();
        }

        private static StringContent EmptyContent
            => CreateContent(new { });

        private static StringContent CreateContent(object obj) 
            => new StringContent(JsonConvert.SerializeObject(obj), encoding: Encoding.UTF8, "application/json");


        private static string GetAddPackUrl(string coffee, int weight) => $"{_baseUrl}/current-user/{coffee}/{weight}";
        private static string GetCreateNewOrderUrl() => $"{_baseUrl}";
    }
}