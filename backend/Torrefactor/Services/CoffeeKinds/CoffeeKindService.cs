using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Torrefactor.DAL;
using Torrefactor.Models;

namespace Torrefactor.Services.CoffeeKinds
{
    public class CoffeeKindService
    {
        private readonly ICoffeeProvider _coffeeProvider;
        private readonly CoffeeKindRepository _coffeeKindRepository;

        public CoffeeKindService(
            ICoffeeProvider coffeeProvider,
            CoffeeKindRepository coffeeKindRepository)
        {
            _coffeeProvider = coffeeProvider;
            _coffeeKindRepository = coffeeKindRepository;
        }

        public Task<IReadOnlyCollection<CoffeeKind>> GetAll() => _coffeeKindRepository.GetAll();

        public async Task ReloadCoffeeKinds()
        {
            var coffeeKinds = (await _coffeeProvider.GetCoffeeKinds())
                .ToLookup(k => k.Name)
                .Select(group =>
                {
                    var allKindsWithSameName = group.ToArray();
                    if (allKindsWithSameName.Length == 1)
                        return allKindsWithSameName[0];

                    var availableCoffeeKinds = allKindsWithSameName
                        .Where(k => k is AvailableCoffeeKind)
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