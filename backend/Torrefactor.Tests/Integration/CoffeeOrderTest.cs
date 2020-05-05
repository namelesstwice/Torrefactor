using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Torrefactor.Core.Services;
using Torrefactor.Models.Coffee;
using Torrefactor.Tests.Integration.Clients;

namespace Torrefactor.Tests.Integration
{
    [TestFixture]
    public class CoffeeOrderTest : BaseIntegrationTest
    {
        private async Task<Connection> SetupServer()
        {
            var connection = new Connection(CreateServer());
            await connection.AuthClient.Register("admin@blah.com", "123", "The Admin");
            await connection.AuthClient.SignIn("admin@blah.com", "123");
            await connection.CoffeeKindClient.ReloadCoffeeKinds();
            await connection.CoffeeOrderClient.CreateNewGroupOrder();
            return connection;
        }

        [Test]
        public async Task Should_add_pack()
        {
            var connection = await SetupServer();

            await connection.CoffeeOrderClient.AddPackToOrder("123", 123);
            var kinds = (await connection.CoffeeKindClient.GetKinds()).Model<IEnumerable<CoffeeKindModel>>();

            CollectionAssert.AreEqual(
                new[] {1},
                kinds.Select(_ => _.Packs.Single(p => p.Weight == 123).Count));
        }

        [Test]
        public async Task Should_not_be_able_to_add_coffee_after_group_order_is_sent()
        {
            var connection = await SetupServer();

            await connection.CoffeeOrderClient.SendToCoffeeProvider();

            Assert.ThrowsAsync<CoffeeOrderException>(async () =>
                await connection.CoffeeOrderClient.AddPackToOrder("123", 123));
        }

        [Test]
        public async Task Should_remove_pack()
        {
            var connection = await SetupServer();

            await connection.CoffeeOrderClient.AddPackToOrder("123", 123);
            await connection.CoffeeOrderClient.RemovePackFromOrder("123", 123);
            var kinds = (await connection.CoffeeKindClient.GetKinds()).Model<IEnumerable<CoffeeKindModel>>();

            CollectionAssert.AreEqual(
                new[] {0},
                kinds.Select(_ => _.Packs.Single(p => p.Weight == 123).Count));
        }
    }
}