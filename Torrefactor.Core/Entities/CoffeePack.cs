using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Core
{
    public class CoffeePack
    {
        private CoffeePack()
        {
            CoffeeKindName = "";
            ExternalId = "";
        }

        [BsonId] public ObjectId Id { get; private set; }

        [BsonElement("state")] [BsonRequired] public CoffeePackState State { get; private set; }

        [BsonElement("weight")] [BsonRequired] public int Weight { get; private set; }

        [BsonElement("price")] [BsonRequired] public int Price { get; private set; }

        [BsonElement("coffeeKindName")]
        [BsonRequired]
        public string CoffeeKindName { get; private set; }

        [BsonElement("externalId")]
        [BsonRequired]
        public string ExternalId { get; private set; }

        [BsonIgnore]
        public decimal PricePer100g => Price / (decimal) Weight * 100;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((CoffeePack) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Weight * 397) ^ (CoffeeKindName != null ? CoffeeKindName.GetHashCode() : 0);
            }
        }

        private bool Equals(CoffeePack other)
        {
            return Weight == other.Weight && string.Equals(CoffeeKindName, other.CoffeeKindName);
        }

        public void Refresh(CoffeeKind coffeeKind)
        {
            if (coffeeKind.Name != CoffeeKindName)
                throw new ArgumentException("Coffee kind name must match.");

            var samePack = coffeeKind.AvailablePacks.SingleOrDefault(p => p.Weight == Weight);
            if (samePack == null)
            {
                MarkAsUnavailable();
                return;
            }

            if (Price != samePack.Price) State = CoffeePackState.PriceChanged;

            Price = samePack.Price;
        }

        public void MarkAsUnavailable()
        {
            State = CoffeePackState.Unavailable;
        }

        public static Builder Create(int weight, int price)
        {
            return new Builder(new CoffeePack
            {
                Weight = weight,
                Price = price
            });
        }

        public class Builder
        {
            private CoffeePack? _pack;

            public Builder(CoffeePack pack)
            {
                _pack = pack;
            }

            public int Weight =>
                _pack?.Weight ?? throw new InvalidOperationException("Build is completed");

            public int Price =>
                _pack?.Price ?? throw new InvalidOperationException("Build is completed");

            public Builder AppendTo(CoffeeKind kind)
            {
                if (_pack == null)
                    throw new InvalidOperationException("Build is completed");

                _pack.CoffeeKindName = kind.Name;
                return this;
            }

            public Builder SetId(string torrefactoId)
            {
                if (_pack == null)
                    throw new InvalidOperationException("Build is completed");

                _pack.ExternalId = torrefactoId;
                return this;
            }

            public CoffeePack Finish()
            {
                var res = _pack ?? throw new InvalidOperationException("Build is completed");

                if (string.IsNullOrEmpty(res.CoffeeKindName))
                    throw new InvalidOperationException("Coffee kind name can't be an empty string");

                if (string.IsNullOrEmpty(res.ExternalId))
                    throw new InvalidOperationException("Torrefacto ID can't be an empty string");

                if (res.Weight < 0)
                    throw new InvalidOperationException("Weight must be positive");

                if (res.Price <= 0)
                    throw new InvalidOperationException("Price must be positive");

                _pack = null;
                return res;
            }
        }
    }
}