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
		public UserController(Config config, CoffeeOrderRepository coffeeOrderRepository)
		{
			_config = config;
			_coffeeOrderRepository = coffeeOrderRepository;
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

			return orders
				.Where(o => o.Packs.Any())
				.Select(o => new
				{
					Name = o.Username,
					Orders = o.Packs
						.GroupBy(p => new
						{
							Coffee = p.CoffeeKindName,
							Weight = p.Weight
						})
						.Select(p => new
						{
							p.Key.Coffee,
							p.Key.Weight,
							Count = p.Count()
						}),
					Price = o.Packs.Sum(_ => _.PriceWithRebate)
				});
		}

		private readonly Config _config;
		private readonly CoffeeOrderRepository _coffeeOrderRepository;
	}
}