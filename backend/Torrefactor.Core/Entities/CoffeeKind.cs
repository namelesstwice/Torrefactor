using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Core
{
    public sealed class CoffeeKind
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local - used by MongoDB driver
        [BsonElement("availablePacks")] private List<CoffeePack> _availablePacks;
        
        // ReSharper disable once UnusedAutoPropertyAccessor.Local - used by MongoDB driver
        [BsonId] public ObjectId Id { get; private set; }
        
        [BsonIgnore] public IReadOnlyCollection<CoffeePack> AvailablePacks => _availablePacks;
        
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - used by MongoDB driver
        [BsonElement("name")] public string Name { get; private set; }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local - used by MongoDB driver
        [BsonElement("isAvailable")] public bool IsAvailable { get; private set; }

        public CoffeeKind(string name, bool isAvailable, IEnumerable<CoffeePack.Builder> coffeePacks)
        {
            Name = name;
            _availablePacks = coffeePacks.Select(p => p.AppendTo(this).Finish()).ToList();
            IsAvailable = isAvailable && _availablePacks.Any();
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