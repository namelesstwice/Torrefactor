using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Services;

namespace Torrefactor.Controllers
{
    [Authorize]
    [Route("api/coffee-kinds")]
    public class CoffeeKindController : Controller
    {
        private readonly CoffeeKindRepository _coffeeKindRepository;
        private readonly CoffeeOrderRepository _coffeeOrderRepository;
        private readonly TorrefactoCoffeeProvider _torrefactoClient;

        public CoffeeKindController(
            CoffeeKindRepository coffeeKindRepository,
            CoffeeOrderRepository coffeeOrderRepository,
            TorrefactoCoffeeProvider torrefactoClient)
        {
            _coffeeKindRepository = coffeeKindRepository;
            _coffeeOrderRepository = coffeeOrderRepository;
            _torrefactoClient = torrefactoClient;
        }

        [HttpGet("")]
        public async Task<IEnumerable<CoffeeOrdersController.CoffeeKindModel>> Get()
        {
            var coffeeKinds = await _coffeeKindRepository.GetAll();
            var userOrders = 
                (await _coffeeOrderRepository.GetUserOrders(User.Identity.Name!)) 
                ?? new CoffeeOrder(User.Identity.Name!);

            return coffeeKinds.Select(kind =>
            {
                var packs = (kind as AvailableCoffeeKind)?.AvailablePacks
                    .Select(pack => new CoffeeOrdersController.CoffeePackModel
                    {
                        Price = pack.Price,
                        Weight = pack.Weight,
                        Count = userOrders.GetCount(kind, pack.Weight)
                    })
                    .OrderBy(_ => _.Weight)
                    .ToArray();
				
                return new CoffeeOrdersController.CoffeeKindModel
                {
                    Name = kind.Name,
                    Packs = packs ?? new CoffeeOrdersController.CoffeePackModel[0],
                    SmallPack = packs?.FirstOrDefault(),
                    BigPack = packs?.Last(),
                    IsAvailable = kind is AvailableCoffeeKind
                };
            });
        }
		
        [HttpPost("reload")]
        [Authorize(Roles = "admin")]
        public async Task ReloadFromTorrefacto()
        {
            var coffeeKinds = (await _torrefactoClient.GetCoffeeKinds())
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