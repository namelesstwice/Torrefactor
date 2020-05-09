using Newtonsoft.Json;
using Torrefactor.Core;

namespace Torrefactor.Models.Coffee
{
    public class CoffeePackModel
    {
        public CoffeePackModel()
        {
        }

        public CoffeePackModel(CoffeePack pack, int count)
        {
            CoffeeKindName = pack.CoffeeKindName;
            Weight = pack.Weight;
            Price = pack.Price;
            Count = count;
        }

        [JsonProperty] public string CoffeeKindName { get; private set; } = "";

        [JsonProperty] public int Weight { get; private set; }

        [JsonProperty] public int Price { get; private set; }

        [JsonProperty] public int Count { get; private set; }
    }
}