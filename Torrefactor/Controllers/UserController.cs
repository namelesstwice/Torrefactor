using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Services;

namespace Torrefactor.Controllers
{
	[Authorize]
	[RoutePrefix("api/users")]
	public class UserController : ApiController
	{
		public UserController(Config config, CoffeeOrderRepository coffeeOrderRepository, CoffeeKindRepository coffeeKindRepository)
		{
			_config = config;
			_coffeeOrderRepository = coffeeOrderRepository;
			_coffeeKindRepository = coffeeKindRepository;
		}

		[Route(""), HttpGet]
		public object Get()
		{
			return new
			{
				Name = User.Identity.Name,
				IsAdmin = User.IsAdmin(_config),
				NoAdmins = !_config.AdminNames.Any()
			};
		}

		[Route("orders"), HttpGet]
		public async Task<IEnumerable> GetOrders()
		{
			if (!User.IsAdmin(_config))
				throw new UnauthorizedAccessException();

			var orders = await _coffeeOrderRepository.GetAll();
			var kinds = (await _coffeeKindRepository.Get(0, 1000)).ToDictionary(p => p.Name);

			foreach (var pack in orders.SelectMany(o => o.Packs))
			{
				CoffeeKind kind;

				if (!kinds.TryGetValue(pack.CoffeeKindName, out kind))
				{
					pack.MarkAsUnavailable();
					continue;
				}

				var availableCoffeKind = kind as AvailableCoffeeKind;
				if (availableCoffeKind == null)
				{
					pack.MarkAsUnavailable();
					continue;
				}

				pack.Refresh(availableCoffeKind);
			}

			return orders
				.Where(o => o.Packs.Any())
				.Select(o => new
				{
					Name = o.Username,
					Orders = o.Packs
						.GroupBy(p => new
						{
							Coffee = p.CoffeeKindName,
							Weight = p.Weight,
							State = p.State
						})
						.Select(p => new
						{
							p.Key.Coffee,
							p.Key.Weight,
							p.Key.State,
							Count = p.Count()
						}),
					OverallState = o.Packs.Any(p => p.State == PackState.Unavailable)
						? PackState.Unavailable
						: o.Packs.Any(p => p.State == PackState.PriceChanged)
							? PackState.PriceChanged 
							: PackState.Available,
					Price = o.Packs.Sum(_ => _.Price)
				});
		}

		private readonly Config _config;
		private readonly CoffeeOrderRepository _coffeeOrderRepository;
		private readonly CoffeeKindRepository _coffeeKindRepository;
	}
}