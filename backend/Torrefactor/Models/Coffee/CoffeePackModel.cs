using Torrefactor.Models;

namespace Torrefactor.Controllers
{
    public class CoffeePackModel
    {
        public int Weight { get; private  set; }
        public int Price { get; private set; }
        public int Count { get; private set; }

        public CoffeePackModel(CoffeePack pack, int count)
        {
            Weight = pack.Weight;
            Price = pack.Weight;
            Count = count;
        }
    }
}