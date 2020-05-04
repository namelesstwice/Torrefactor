using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using AspNetCore.Identity.Mongo;
using AspNetCore.Identity.Mongo.Stores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Newtonsoft.Json;
using Torrefactor.Models;
using Torrefactor.Models.Auth;
using Torrefactor.Services;
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
                        AdminEmails = new List<string> { "admin@blah.com" },
                        Secret = "a very secret string that is definitely not used in production"
                    };
                    
                    services.AddSingleton(fakeCfg);

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