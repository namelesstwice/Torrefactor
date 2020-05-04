using System.Collections;
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
	[Route("api/coffee-orders")]
	public class CoffeeOrdersController : Controller
	{
		public CoffeeOrdersController(
			CoffeeKindRepository coffeeKindRepository, 
			CoffeeOrderRepository coffeeOrderRepository,
			CoffeeOrderService coffeeOrderService)
		{
			_coffeeKindRepository = coffeeKindRepository;
			_coffeeOrderRepository = coffeeOrderRepository;
			_coffeeOrderService = coffeeOrderService;
		}

		[HttpGet("orders")]
		[Authorize(Roles = "admin")]
		public async Task<IEnumerable> GetAllOrders()
		{
			var orders = await _coffeeOrderRepository.GetAll();
			var kinds = (await _coffeeKindRepository.GetAll()).ToDictionary(p => p.Name);

			foreach (var pack in orders.SelectMany(o => o.Packs))
			{
				if (!kinds.TryGetValue(pack.CoffeeKindName, out var kind))
				{
					pack.MarkAsUnavailable();
					continue;
				}

				if (!(kind is AvailableCoffeeKind availableCoffeeKind))
				{
					pack.MarkAsUnavailable();
					continue;
				}

				pack.Refresh(availableCoffeeKind);
			}

			return orders
				.Where(o => o.Packs.Any())
				.Select(o => new
				{
					Name = o.Username,
					Orders = o.Packs
						.GroupBy(p => new
						{
							CoffeeName = p.CoffeeKindName,
							Weight = p.Weight,
							State = p.State
						})
						.Select(p => new
						{
							Name = p.Key.CoffeeName,
							Packs = new []
							{
								new
								{
									Weight = p.Key.Weight,
									Count = p.Count()
								}
							},
						}),
					OverallState = o.Packs.Any(p => p.State == PackState.Unavailable)
						? PackState.Unavailable
						: o.Packs.Any(p => p.State == PackState.PriceChanged)
							? PackState.PriceChanged 
							: PackState.Available,
					Price = o.Packs.Sum(_ => _.Price)
				});
		}

		[HttpPost("add")]
		public async Task AddPackToOrder(string coffeeName, int weight)
		{
			await _coffeeOrderService.AddPackToOrder(User.Identity.Name!, coffeeName, weight);
		}

		[HttpPost("remove")]
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

		[HttpPost("clear")]
		[Authorize(Roles = "admin")]
		public async Task ClearAllOrders()
		{
			await _coffeeOrderRepository.Clean();
		}

		private readonly CoffeeKindRepository _coffeeKindRepository;
		private readonly CoffeeOrderRepository _coffeeOrderRepository;
		private readonly CoffeeOrderService _coffeeOrderService;
	}
}