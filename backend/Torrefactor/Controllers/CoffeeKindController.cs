using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torrefactor.Core;
using Torrefactor.Core.Services;
using Torrefactor.Models.Coffee;

namespace Torrefactor.Controllers
{
    [Authorize]
    [Route("api/coffee-kinds")]
    public class CoffeeKindController : Controller
    {
        private readonly CoffeeKindService _coffeeKindService;
        private readonly CoffeeOrderService _coffeeOrderService;

        public CoffeeKindController(
            CoffeeOrderService coffeeOrderService,
            CoffeeKindService coffeeKindService)
        {
            _coffeeOrderService = coffeeOrderService;
            _coffeeKindService = coffeeKindService;
        }

        [HttpGet("")]
        public async Task<IEnumerable<CoffeeKindModel>> Get()
        {
            var coffeeKinds = await _coffeeKindService.GetAll();
            var userOrder = await _coffeeOrderService.TryGetPersonalOrder(User.Identity.Name!);

            return coffeeKinds.Select(kind =>
            {
                var packs = kind.AvailablePacks
                    .Select(pack => new CoffeePackModel(pack, userOrder?.GetCount(kind, pack.Weight) ?? 0))
                    .OrderBy(_ => _.Weight)
                    .ToArray();

                return new CoffeeKindModel(
                    kind.Name,
                    packs,
                    kind.IsAvailable,
                    packs?.First(),
                    packs?.Last());
            });
        }

        [HttpPost("reload")]
        [Authorize(Roles = "admin")]
        public async Task ReloadCoffeeKinds()
        {
            await _coffeeKindService.ReloadCoffeeKinds();
        }
    }
}