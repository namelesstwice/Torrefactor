using System;
using System.Collections;
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
        public async Task<GroupCoffeeOrderModel> GetCurrentGroupOrder()
        {
            var currentOrder = await _coffeeOrderService.TryGetCurrentGroupOrder();
            if (currentOrder == null)
                return new GroupCoffeeOrderModel(false);

            var personalOrders = currentOrder.PersonalOrders
                .Where(o => o.Packs.Any())
                .Select(o => new PersonalCoffeeOrderModel(o));
            
            return new GroupCoffeeOrderModel(true, personalOrders);
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
        public async Task CreateNewGroupOrder([FromQuery] string? providerId)
        {
            await _coffeeOrderService.CreateNewGroupOrder(
                providerId ?? throw new ArgumentNullException(nameof(providerId)));
        }
    }
}