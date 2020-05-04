using System.Threading.Tasks;
using NUnit.Framework;
using Torrefactor.Tests.Integration.Clients;

namespace Torrefactor.Tests.Integration
{
    [TestFixture]
    public class CoffeeOrderTest : BaseIntegrationTest
    {
        [Test]
        public async Task Should_add_pack()
        {
            var connection = new Connection(CreateServer());
            await connection.AuthClient.Register("admin@blah.com", "123", "The Admin");
            await connection.AuthClient.SignIn("admin@blah.com", "123");

            await connection.CoffeeOrderClient.CreateNewGroupOrder();
            await connection.CoffeeOrderClient.AddPackToOrder("123", 123);
        }
    }
}