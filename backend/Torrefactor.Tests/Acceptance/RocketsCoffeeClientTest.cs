using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Torrefactor.Core;
using Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee;

namespace Torrefactor.Tests.Acceptance
{
    [TestFixture, Explicit]
    public class RocketsCoffeeClientTest
    {
        [Test]
        public async Task Authenticate()
        {
            var client = new RocketsCoffeeClient();
            await client.Authenticate("");
            await client.Authenticate("123");
            var kind = new CoffeeKind("123", true, new[] {CoffeePack.Create(123, 123).SetId("1227")});
            await client.AddToBasket(kind, kind.AvailablePacks.Single(), 1);
        }
    }
}