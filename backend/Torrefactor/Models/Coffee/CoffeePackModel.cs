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

        public string CoffeeKindName { get; private set; } = "";

        public int Weight { get; private set; }

        public int Price { get; private set; }

        public int Count { get; private set; }
    }
}