using System.Collections.Generic;
using System.Linq;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public class CoffeeRoasterSelector : ICoffeeRoasterSelector
    {
        private readonly IReadOnlyDictionary<string, ICoffeeRoasterClient> _coffeeProviders;

        public CoffeeRoasterSelector(IEnumerable<ICoffeeRoasterClient> coffeeProviders)
        {
            _coffeeProviders = coffeeProviders.ToDictionary(_ => _.Id);
        }

        public IReadOnlyCollection<string> GetProviderIds()
            => _coffeeProviders.Keys.ToList();

        public ICoffeeRoasterClient SelectFor(GroupCoffeeOrder order)
            => _coffeeProviders[order.ProviderId];
    }
}