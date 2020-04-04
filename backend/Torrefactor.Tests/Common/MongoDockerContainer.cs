using MongoDB.Driver;

namespace Torrefactor.Tests.Common
{
    public class MongoDockerContainer : DockerContainer<IMongoClient>
    {
        protected override IMongoClient CreateConnection()
            => new MongoClient(ConnectionString);
        protected override string Image { get; } = "mongo";
        protected override string Tag { get; } = "4.2";
        protected override string ContainerPort { get; } = "27017";
        public string ConnectionString => $"mongodb://localhost:{HostPort}";
    }
}