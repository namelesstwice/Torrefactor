using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Core
{
    public class PersonalCoffeeOrder
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local - used by MongoDB driver
        [BsonElement("packs")] private List<CoffeePack> _packs;

        public PersonalCoffeeOrder(string username)
        {
            Username = username;
            _packs = new List<CoffeePack>();
        }

        [BsonId] public string Username { get; }

        [BsonIgnore] public IReadOnlyCollection<CoffeePack> Packs => _packs;

        public int GetCount(CoffeeKind kind, int weight) 
            => _packs.Count(_ => _.CoffeeKindName == kind.Name && _.Weight == weight);

        public IEnumerable<(CoffeePack Pack, int Count)> GetUniquePacksCount()
            => from p in _packs
                group p by (p.CoffeeKindName, p.Weight) into g
                select (g.First(), g.Count());

        public void AddCoffeePack(CoffeePack pack)
        {
            _packs.Add(pack);
        }

        public void RemoveCoffeePack(CoffeePack pack)
        {
            _packs.Remove(pack);
        }
    }
}