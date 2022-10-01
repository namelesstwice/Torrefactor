using System.Collections.Generic;
using System.Linq;
using Torrefactor.Core;

namespace Torrefactor.Models.Coffee
{
    public class PersonalCoffeeOrderModel
    {
        public string Username { get; }
        
        public IEnumerable<CoffeePackModel> CoffeePacks { get; }
        
        public CoffeePackState OverallState { get; }
        
        public int TotalCost { get; }

        public PersonalCoffeeOrderModel(PersonalCoffeeOrder order)
        {
            Username = order.Username;
            CoffeePacks = order.GetUniquePacksCount().Select(_ => new CoffeePackModel(_.Pack, _.Count));
            OverallState = order.Packs.Any(p => p.State == CoffeePackState.Unavailable)
                ? CoffeePackState.Unavailable
                : order.Packs.Any(p => p.State == CoffeePackState.PriceChanged)
                    ? CoffeePackState.PriceChanged
                    : CoffeePackState.Available;
            TotalCost = order.Packs.Sum(_ => _.Price);
        }
    }
}