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