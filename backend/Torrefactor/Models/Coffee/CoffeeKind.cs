using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
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

		[BsonElement("name")]
		public string Name { get; private set; }
	}
}