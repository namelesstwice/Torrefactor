using Microsoft.AspNetCore.TestHost;

namespace Torrefactor.Tests.Integration.Clients
{
    public class Connection
    {
        public AuthClient AuthClient { get; private set; }
        
        public CoffeeOrderClient CoffeeOrderClient { get; private set; }

        public Connection(TestServer testServer)
        {
            var client = testServer.CreateClient();
            AuthClient = new AuthClient(client);
            CoffeeOrderClient = new CoffeeOrderClient(client);
        }
    }
}