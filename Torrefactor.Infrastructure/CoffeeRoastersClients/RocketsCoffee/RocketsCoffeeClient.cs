using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee
{
    internal sealed class RocketsCoffeeClient : ICoffeeRoasterClient
    {
        private const string BaseUrl = "https://rockets.coffee";

        public string Id { get; } = "Rockets";

        public async Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync($"{BaseUrl}/catalog/coffee");
            
            return RocketsCoffeeListPageParser
                .Parse(await response.Content.ReadAsStringAsync())
                .ToList();
        }

        public Task Authenticate()
        {
            throw new System.NotImplementedException();
        }

        public Task CleanupBasket()
        {
            throw new System.NotImplementedException();
        }

        public Task AddToBasket(CoffeeKind kind, CoffeePack pack, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}