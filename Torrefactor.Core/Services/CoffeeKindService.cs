using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public class CoffeeKindService
    {
        private readonly ICoffeeKindRepository _coffeeKindRepository;
        private readonly IGroupCoffeeOrderRepository _groupCoffeeOrderRepository;
        private readonly ICoffeeRoasterSelector _coffeeRoasterSelector;

        public CoffeeKindService(
            ICoffeeKindRepository coffeeKindRepository, 
            IGroupCoffeeOrderRepository groupCoffeeOrderRepository,
            ICoffeeRoasterSelector coffeeRoasterSelector)
        {
            _coffeeKindRepository = coffeeKindRepository;
            _groupCoffeeOrderRepository = groupCoffeeOrderRepository;
            _coffeeRoasterSelector = coffeeRoasterSelector;
        }

        public Task<IReadOnlyCollection<CoffeeKind>> GetAll()
        {
            return _coffeeKindRepository.GetAll();
        }

        public async Task ReloadCoffeeKinds()
        {
            var groupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (groupOrder == null)
                throw new CoffeeOrderException("Can't reload coffee kinds, active order not found");

            var coffeeProvider = _coffeeRoasterSelector.SelectFor(groupOrder);
            
            var coffeeKinds = (await coffeeProvider.GetCoffeeKinds())
                .ToLookup(k => k.Name)
                .Select(group =>
                {
                    var allKindsWithSameName = group.ToArray();
                    if (allKindsWithSameName.Length == 1)
                        return allKindsWithSameName[0];

                    var availableCoffeeKinds = allKindsWithSameName
                        .Where(k => k.IsAvailable)
                        .ToArray();

                    if (availableCoffeeKinds.Length == 1)
                        return availableCoffeeKinds[0];

                    if (availableCoffeeKinds.Length == 0)
                        return null;

                    throw new InvalidOperationException(
                        "Unexpected coffee kind count with name: " + group.Key);
                })
                .Where(k => k != null)
                .ToArray();

            await _coffeeKindRepository.Clean();
            await _coffeeKindRepository.Insert(coffeeKinds!);
        }
    }
}