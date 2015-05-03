using System;

namespace Torrefactor.Models
{
	public class CoffeePack
	{
		protected bool Equals(CoffeePack other)
		{
			return Weight == other.Weight && string.Equals(CoffeeKindName, other.CoffeeKindName);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CoffeePack) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Weight*397) ^ (CoffeeKindName != null ? CoffeeKindName.GetHashCode() : 0);
			}
		}

		private CoffeePack()
		{
		}

		public int Weight { get; private set; }

		public int Price { get; private set; }

		public int PriceWithRebate
		{
			get { return (int) Math.Round(Price*0.85); }
		}

		public string CoffeeKindName { get; private set; }

		public string TorrefactoId { get; private set; }

		public static CoffeePack.Builder Create(int weight, int price)
		{
			return new Builder(new CoffeePack
			{
				Weight = weight,
				Price = price
			});
		}

		public class Builder
		{
			private CoffeePack _pack;

			public int Weight
			{
				get { return _pack.Weight; }
			}

			public Builder(CoffeePack pack)
			{
				_pack = pack;
			}

			public CoffeePack.Builder AppendTo(CoffeeKind kind)
			{
				_pack.CoffeeKindName = kind.Name;
				return this;
			}

			public CoffeePack.Builder SetId(string torrefactoId)
			{
				_pack.TorrefactoId = torrefactoId;
				return this;
			}

			public CoffeePack Finish()
			{
				var res = _pack;

				if (string.IsNullOrEmpty(res.CoffeeKindName))
					throw new InvalidOperationException("Coffee kind name can't be an empty string");

				if (string.IsNullOrEmpty(res.TorrefactoId))
					throw new InvalidOperationException("Torrefacto ID can't be an empty string");

				if (res.Weight <= 0)
					throw new InvalidOperationException("Weight must be positive");

				if (res.Price <= 0)
					throw new InvalidOperationException("Price must be positive");

				_pack = null;
				return res;
			}
		}
	}
}