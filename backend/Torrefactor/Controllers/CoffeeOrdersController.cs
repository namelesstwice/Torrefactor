using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Torrefactor.Core;
using Torrefactor.Core.Services;

namespace Torrefactor.Controllers
{
    [Authorize]
    [Route("api/coffee-orders")]
    public class CoffeeOrdersController : Controller
    {
        private readonly CoffeeKindService _coffeeKindService;
        private readonly CoffeeOrderService _coffeeOrderService;

        public CoffeeOrdersController(
            CoffeeKindService coffeeKindService,
            CoffeeOrderService coffeeOrderService)
        {
            _coffeeKindService = coffeeKindService;
            _coffeeOrderService = coffeeOrderService;
        }

        [HttpGet("")]
        [Authorize(Roles = "admin")]
        public async Task<IEnumerable> GetAllOrders()
        {
            var currentOrder = await _coffeeOrderService.TryGetCurrentGroupOrder();
            if (currentOrder == null)
                return Enumerable.Empty<object>();

            var kinds = (await _coffeeKindService.GetAll()).ToDictionary(p => p.Name);

            foreach (var pack in currentOrder.PersonalOrders.SelectMany(o => o.Packs))
            {
                if (!kinds.TryGetValue(pack.CoffeeKindName, out var kind))
                {
                    pack.MarkAsUnavailable();
                    continue;
                }

                if (!kind.IsAvailable)
                {
                    pack.MarkAsUnavailable();
                    continue;
                }

                pack.Refresh(kind);
            }

            return currentOrder.PersonalOrders
                .Where(o => o.Packs.Any())
                .Select(o => new
                {
                    Name = o.Username,
                    Orders = o.Packs
                        .GroupBy(p => new
                        {
                            CoffeeName = p.CoffeeKindName,
                            p.Weight,
                            p.State
                        })
                        .Select(p => new
                        {
                            Name = p.Key.CoffeeName,
                            Packs = new[]
                            {
                                new
                                {
                                    p.Key.Weight,
                                    Count = p.Count()
                                }
                            }
                        }),
                    OverallState = o.Packs.Any(p => p.State == CoffeePackState.Unavailable)
                        ? CoffeePackState.Unavailable
                        : o.Packs.Any(p => p.State == CoffeePackState.PriceChanged)
                            ? CoffeePackState.PriceChanged
                            : CoffeePackState.Available,
                    Price = o.Packs.Sum(_ => _.Price)
                });
        }

        [HttpPost("current-user/{coffeeName}/{weight}")]
        public async Task AddPackToOrder(string coffeeName, int weight)
        {
            await _coffeeOrderService.AddPackToOrder(User.Identity.Name!, coffeeName, weight);
        }

        [HttpDelete("current-user/{coffeeName}/{weight}")]
        public async Task RemovePackFromOrder(string coffeeName, int weight)
        {
            await _coffeeOrderService.RemovePackFromOrder(User.Identity.Name!, coffeeName, weight);
        }

        [HttpPost("send")]
        [Authorize(Roles = "admin")]
        public async Task SendToCoffeeProvider()
        {
            await _coffeeOrderService.SendToCoffeeProvider();
        }


        [HttpPost("")]
        [Authorize(Roles = "admin")]
        public async Task CreateNewGroupOrder()
        {
            await _coffeeOrderService.CreateNewGroupOrder();
        }
    }
}