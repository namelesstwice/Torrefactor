using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
	[BsonDiscriminator("orderableCoffeeKind")]
	public sealed class AvailableCoffeeKind : CoffeeKind
	{
		private List<CoffeePack> _availablePacks;

		private AvailableCoffeeKind(int torrefactoId)
		{
			TorrefactoId = torrefactoId;
		}

		public AvailableCoffeeKind(string name, int torrefactoId, IEnumerable<CoffeePack.Builder> coffeePacks)
			: base(name)
		{
			TorrefactoId = torrefactoId;
			AvailablePacks = coffeePacks.Select(p => p.AppendTo(this).Finish()).ToArray();
		}
		
		[BsonElement("torrefactoId")]
		public int TorrefactoId { get; private set; }

		[BsonElement("availablePacks")]
		public IReadOnlyCollection<CoffeePack> AvailablePacks
		{
			get { return _availablePacks; }
			private set { _availablePacks = new List<CoffeePack>(value); }
		}

		public string GetActualTorrefactoId(CoffeePack pack)
		{
			var ix = _availablePacks.IndexOf(pack);
			if (ix == -1)
				throw new ArgumentException("Unknown coffee pack: " + pack);

			pack = _availablePacks[ix];
			return pack.TorrefactoId;
		}
	}
}