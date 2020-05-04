using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
	public class CoffeeOrder
	{
		[BsonElement("packs")]
		private List<CoffeePack> _packs;
		
		[BsonId]
		public string Username { get; private set; }
		
		[BsonElement("groupOrderId")]
		public ObjectId GroupOrderId { get; private set; }
		
		[BsonIgnore]
		public IReadOnlyCollection<CoffeePack> Packs => _packs;

		public CoffeeOrder(string username, ObjectId groupOrderId)
		{
			Username = username;
			GroupOrderId = groupOrderId;
			_packs = new List<CoffeePack>();
		}

		public void AddCoffeePack(CoffeePack pack)
		{
			_packs.Add(pack);
		}

		public void RemoveCoffeePack(CoffeePack pack)
		{
			_packs.Remove(pack);
		}

		public int GetCount(CoffeeKind kind, int weight)
		{
			return _packs.Count(_ => _.CoffeeKindName == kind.Name && _.Weight == weight);
		}
	}
}