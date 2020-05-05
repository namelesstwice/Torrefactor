using System.Collections.Generic;
using System.Linq;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public class CoffeeProviderSelector : ICoffeeProviderSelector
    {
        private readonly IReadOnlyDictionary<string, ICoffeeProvider> _coffeeProviders;

        public CoffeeProviderSelector(IEnumerable<ICoffeeProvider> coffeeProviders)
        {
            _coffeeProviders = coffeeProviders.ToDictionary(_ => _.Id);
        }

        public ICoffeeProvider SelectFor(GroupCoffeeOrder order)
            => _coffeeProviders[order.ProviderId];
    }
}