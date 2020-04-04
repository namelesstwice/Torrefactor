using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using NUnit.Framework;

namespace Torrefactor.Tests.Common
{
    public abstract class BaseMongoTest
    {
        [OneTimeSetUp]
        public async Task OneTimeSetup()
        {
            await _mongoContainer.Run();
        }

        [SetUp]
        public void Setup()
        {
            MongoDatabase = _mongoContainer.Connect().GetDatabase($"tempdb-{Guid.NewGuid()}");
            ConnectionString = _mongoContainer.ConnectionString;
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _mongoContainer.StopAndRemove();
        }

        protected IMongoDatabase MongoDatabase { get; private set; }
        protected string ConnectionString { get; private set; }

        private readonly MongoDockerContainer _mongoContainer = new MongoDockerContainer();
    }
}