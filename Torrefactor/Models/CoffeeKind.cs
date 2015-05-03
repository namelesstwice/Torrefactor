using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
	[BsonDiscriminator("coffeeKind")]
	[BsonKnownTypes(typeof(AvailableCoffeeKind))]
	public class CoffeeKind
	{
		[BsonId]
		private ObjectId _id;
		
		public CoffeeKind(string name)
		{
			Name = name;
		}

		protected CoffeeKind()
		{
		}

		[BsonElement("name")]
		public string Name { get; private set; }
	}
}