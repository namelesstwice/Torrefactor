using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        private readonly ICoffeeRoasterSelector _coffeeRoasterSelector;

        public CoffeeKindController(
            CoffeeOrderService coffeeOrderService,
            CoffeeKindService coffeeKindService, 
            ICoffeeRoasterSelector coffeeRoasterSelector)
        {
            _coffeeOrderService = coffeeOrderService;
            _coffeeKindService = coffeeKindService;
            _coffeeRoasterSelector = coffeeRoasterSelector;
        }

        [HttpGet("")]
        public async Task<CoffeeKindPageModel> Get()
        {
            var (userOrder, groupOrderExists) = await _coffeeOrderService.TryGetPersonalOrder(User.Identity.Name!);
            if (!groupOrderExists)
                return new CoffeeKindPageModel(false, new CoffeeKindModel[0]);
            
            var coffeeKinds = await _coffeeKindService.GetAll();

            var kinds = 
                from k in coffeeKinds
                where k.IsAvailable
                let packs = (
                    from p in k.AvailablePacks
                    orderby p.Weight
                    select new CoffeePackModel(p, userOrder?.GetCount(k, p.Weight) ?? 0))
                    .ToArray()
                select new CoffeeKindModel(k.Name, packs, k.IsAvailable, packs?.First(), packs?.Last());
            
            return new CoffeeKindPageModel(groupOrderExists, kinds);
        }

        [HttpGet("roasters")]
        [Authorize(Roles = "admin")]
        public IEnumerable<CoffeeRoasterModel> GetCoffeeRoasters()
        {
            return _coffeeRoasterSelector.GetRoasters().Select(_ => new CoffeeRoasterModel(_.Id, _.Name));
        }

        [HttpPost("reload")]
        [Authorize(Roles = "admin")]
        public async Task ReloadCoffeeKinds()
        {
            await _coffeeKindService.ReloadCoffeeKinds();
        }
    }
}