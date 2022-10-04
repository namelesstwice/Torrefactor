using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee
{
    public sealed class RocketsCoffeeClient : ICoffeeRoasterClient
    {
        private const string BaseUrl = "https://rockets.coffee";
        private readonly CookieContainer _container;
        private readonly HttpClient _httpClient;

        public RocketsCoffeeClient()
        {
            _container = new CookieContainer();
            _httpClient = new HttpClient(new HttpClientHandler() { CookieContainer = _container});
        }

        public CoffeeRoaster Roaster { get; } = new CoffeeRoaster("Rockets", "Rockets coffee");

        public async Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds()
        {
            var response = await _httpClient.GetAsync($"{BaseUrl}/catalog/coffee");
            
            return RocketsCoffeeListPageParser
                .Parse(await response.Content.ReadAsStringAsync())
                .ToList();
        }

        public async Task Authenticate(string key)
        {
            _container.Add(new Uri(BaseUrl), new Cookie("PHPSESSID", key));
        }

        public async Task CleanupBasket()
        {
            
        }

        public async Task AddToBasket(CoffeeKind kind, CoffeePack pack, int count)
        {
            
            await _httpClient.GetAsync($"{BaseUrl}/ajax/cart.php?variant={pack.ExternalId}&amount={count}");
        }
    }
}