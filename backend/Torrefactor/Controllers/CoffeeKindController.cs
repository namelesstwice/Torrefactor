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
        public async Task<CoffeeKindPageModel> Get()
        {
            var coffeeKinds = await _coffeeKindService.GetAll();
            var (userOrder, groupOrderExists) = await _coffeeOrderService.TryGetPersonalOrder(User.Identity.Name!);

            var kinds = 
                from k in coffeeKinds
                let packs = (
                    from p in k.AvailablePacks
                    orderby p.Weight
                    select new CoffeePackModel(p, userOrder?.GetCount(k, p.Weight) ?? 0))
                    .ToArray()
                select new CoffeeKindModel(k.Name, packs, k.IsAvailable, packs?.First(), packs?.Last());
            
            return new CoffeeKindPageModel(groupOrderExists, kinds);
        }

        [HttpPost("reload")]
        [Authorize(Roles = "admin")]
        public async Task ReloadCoffeeKinds()
        {
            await _coffeeKindService.ReloadCoffeeKinds();
        }
    }
}