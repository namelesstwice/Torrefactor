using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Core
{
    [BsonDiscriminator("coffeeKind")]
    [BsonKnownTypes(typeof(AvailableCoffeeKind))]
    public class CoffeeKind
    {
        [BsonId]
#pragma warning disable 169
        private ObjectId _id;
#pragma warning restore 169

        public CoffeeKind(string name)
        {
            Name = name;
        }

        protected CoffeeKind()
        {
            Name = "";
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - used by MongoDB driver
        [BsonElement("name")] public string Name { get; private set; }
    }
}