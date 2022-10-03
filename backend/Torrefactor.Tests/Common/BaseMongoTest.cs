using System;
using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using MongoDB.Driver;
using NUnit.Framework;

namespace Torrefactor.Tests.Common
{
    public abstract class BaseMongoTest
    {
        private readonly MongoDbTestcontainer _mongoContainer = new TestcontainersBuilder<MongoDbTestcontainer>()
            .WithDatabase(new MongoDbTestcontainerConfiguration()
            {
                Database = "tst",
                Username = "tst",
                Password = "tst"
            })
            .Build();

        protected IMongoDatabase MongoDatabase { get; private set; }
        protected string ConnectionString { get; private set; }

        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _mongoContainer.StartAsync();
        }

        [SetUp]
        public void Setup()
        {
            var client = new MongoClient(_mongoContainer.ConnectionString);
            MongoDatabase = client.GetDatabase($"tempdb-{Guid.NewGuid()}");
            ConnectionString = _mongoContainer.ConnectionString;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _mongoContainer.StopAsync();
            await _mongoContainer.CleanUpAsync();
        }
    }
}