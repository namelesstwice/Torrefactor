using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
	public class CoffeeOrder
	{
		[BsonElement("packs")]
		private List<CoffeePack> _packs;

		public CoffeeOrder(string username)
		{
			Username = username;
			_packs = new List<CoffeePack>();
		}

		[BsonId]
		public string Username { get; private set; }

		public IReadOnlyCollection<CoffeePack> Packs
		{
			get { return _packs; }
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