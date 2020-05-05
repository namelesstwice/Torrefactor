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
            Weight = pack.Weight;
            Price = pack.Weight;
            Count = count;
        }

        [JsonProperty] public int Weight { get; private set; }

        [JsonProperty] public int Price { get; private set; }

        [JsonProperty] public int Count { get; private set; }
    }
}