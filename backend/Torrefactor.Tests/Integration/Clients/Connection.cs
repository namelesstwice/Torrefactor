using Microsoft.AspNetCore.TestHost;

namespace Torrefactor.Tests.Integration.Clients
{
    public class Connection
    {
        public Connection(TestServer testServer)
        {
            var httpClient = testServer.CreateClient();
            AuthClient = new AuthClient(httpClient);
            CoffeeOrderClient = new CoffeeOrderClient(httpClient);
            CoffeeKindClient = new CoffeeKindClient(httpClient);
        }

        public AuthClient AuthClient { get; }

        public CoffeeOrderClient CoffeeOrderClient { get; }
        public CoffeeKindClient CoffeeKindClient { get; }
    }
}