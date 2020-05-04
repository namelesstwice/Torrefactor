using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
	[BsonDiscriminator("availableCoffeeKind")]
	public sealed class AvailableCoffeeKind : CoffeeKind
	{
		private List<CoffeePack> _availablePacks;

		private AvailableCoffeeKind(int externalId)
		{
			ExternalId = externalId;
			_availablePacks = new List<CoffeePack>();
		}

		public AvailableCoffeeKind(string name, int externalId, IEnumerable<CoffeePack.Builder> coffeePacks)
			: base(name)
		{
			ExternalId = externalId;
			_availablePacks = coffeePacks.Select(p => p.AppendTo(this).Finish()).ToList();
		}
		
		[BsonElement("externalId")]
		public int ExternalId { get; private set; }

		[BsonElement("availablePacks")]
		public IReadOnlyCollection<CoffeePack> AvailablePacks
		{
			get => _availablePacks;
			private set => _availablePacks = new List<CoffeePack>(value);
		}

		public string GetActualExternalId(CoffeePack pack)
		{
			var ix = _availablePacks.IndexOf(pack);
			if (ix == -1)
				throw new ArgumentException("Unknown coffee pack: " + pack);

			pack = _availablePacks[ix];
			return pack.ExternalId;
		}
	}
}