using System.Collections.Generic;
using AspNetCore.Identity.Mongo.Stores;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;
using Torrefactor.Entities.Auth;
using Torrefactor.Infrastructure;
using Torrefactor.Tests.Common;

namespace Torrefactor.Tests.Integration
{
    public class BaseIntegrationTest : BaseMongoTest
    {
        public TestServer CreateServer()
        {
            var hostBuilder = new WebHostBuilder()
                .ConfigureTestServices(services =>
                {
                    var fakeCfg = new Config
                    {
                        MongodbConnectionString = ConnectionString,
                        DatabaseName = MongoDatabase.DatabaseNamespace.DatabaseName,
                        AdminEmails = new List<string> {"admin@blah.com"},
                        Secret = "a very secret string that is definitely not used in production"
                    };

                    var fakeCoffeeProvider = A.Fake<ICoffeeProvider>();
                    A.CallTo(() => fakeCoffeeProvider.GetCoffeeKinds()).Returns(new[]
                    {
                        new CoffeeKind("123", true, new[]
                        {
                            CoffeePack.Create(123, 123).SetId("123")
                        })
                    });

                    services.AddSingleton(fakeCfg);
                    services.AddSingleton(fakeCoffeeProvider);

                    // TODO: hack, need to modify AspNetCore.Identity.Mongo
                    var userCollection = MongoDatabase.GetCollection<ApplicationUser>("Users");
                    var roleCollection = MongoDatabase.GetCollection<ApplicationRole>("Roles");
                    services.AddSingleton(userCollection);
                    services.AddSingleton(roleCollection);
                    services.AddTransient<IRoleStore<ApplicationRole>>(p =>
                        new RoleStore<ApplicationRole>(roleCollection));
                    services.AddTransient<IUserStore<ApplicationUser>>(
                        p => new UserStore<ApplicationUser, ApplicationRole>(
                            userCollection,
                            p.GetService<IRoleStore<ApplicationRole>>(),
                            p.GetService<ILookupNormalizer>()));
                })
                .UseStartup<Startup>();

            var testServer = new TestServer(hostBuilder);

            return testServer;
        }
    }
}