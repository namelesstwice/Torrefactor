using System;
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
			ChangeStamp = Guid.NewGuid();
		}

		public CoffeeOrder(CoffeeOrder userOrders)
		{
			Username = userOrders.Username;
			_packs = userOrders.Packs.ToList();
			ChangeStamp = Guid.NewGuid();
		}

		[BsonId]
		public string Username { get; private set; }

		[BsonElement("changeStamp")]
		public Guid ChangeStamp { get; private set; }

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