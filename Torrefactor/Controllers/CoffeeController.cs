using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Services;

namespace Torrefactor.Controllers
{
	[Authorize]
	[RoutePrefix("api/coffee")]
	public class CoffeeController : ApiController
	{
		public CoffeeController(
			CoffeeKindRepository coffeeKindRepository, 
			CoffeeOrderRepository coffeeOrderRepository,
			TorrefactoClient torrefactoClient,
			Config config)
		{
			_coffeeKindRepository = coffeeKindRepository;
			_coffeeOrderRepository = coffeeOrderRepository;
			_torrefactoClient = torrefactoClient;
			_config = config;
		}

		[Route(""), HttpGet]
		public async Task<IEnumerable<CoffeeKindModel>> Get()
		{
			var coffeeKinds = await _coffeeKindRepository.GetAll();
			var userOrders = 
				(await _coffeeOrderRepository.GetUserOrders(User.Identity.Name)) 
				?? new CoffeeOrder(User.Identity.Name);

			return coffeeKinds.Select(kind =>
				new CoffeeKindModel
				{
					Name = kind.Name,
					Packs = kind is AvailableCoffeeKind
						? ((AvailableCoffeeKind) kind).AvailablePacks
							.Select(pack => new CoffeePackModel
							{
								Price = pack.PriceWithRebate,
								Weight = pack.Weight,
								Count = userOrders.GetCount(kind, pack.Weight)
							})
							.ToArray()
						: null,
					IsAvailable = kind is AvailableCoffeeKind
				});
		}

		[Route("add"), HttpPost]
		public async Task Add(string coffeeName, int weight)
		{
			var userOrders =
				(await _coffeeOrderRepository.GetUserOrders(User.Identity.Name))
				?? new CoffeeOrder(User.Identity.Name);

			var desiredPack = await getDesiredPack(coffeeName, weight);
			userOrders.AddCoffeePack(desiredPack);

			await _coffeeOrderRepository.Update(userOrders, upsert:true);
		}

		[Route("remove"), HttpPost]
		public async Task Remove(string coffeeName, int weight)
		{
			var userOrders = await _coffeeOrderRepository.GetUserOrders(User.Identity.Name);
			if (userOrders == null)
				throw new ArgumentException(nameof(userOrders));

			var desiredPack = await getDesiredPack(coffeeName, weight);
			if (!userOrders.Packs.Contains(desiredPack))
				throw new ArgumentException("attempt to remove not added coffee");

			userOrders.RemoveCoffeePack(desiredPack);

			await _coffeeOrderRepository.Update(userOrders);
		}

		[Route("reload"), HttpPost]
		public async Task ReloadFromTorrefacto()
		{
			if (!User.IsAdmin(_config))
				throw new UnauthorizedAccessException();

			var coffeeKinds = (await _torrefactoClient.GetCoffeeKinds())
				.ToLookup(k => k.Name)
				.Select(group =>
				{
					var allKindsWithSameName = group.ToArray();
					if (allKindsWithSameName.Length == 1)
						return allKindsWithSameName[0];

					var availableCoffeKinds = allKindsWithSameName
						.Where(k => k is AvailableCoffeeKind)
						.ToArray();

					if (availableCoffeKinds.Length == 1)
						return availableCoffeKinds[0];

					if (availableCoffeKinds.Length == 0)
						return null;

					throw new InvalidOperationException(
						"Unexpeted coffee kind count with name: " + group.Key);
				})
				.Where(k => k != null)
				.ToArray();

			await _coffeeKindRepository.Clean();
			await _coffeeKindRepository.Insert(coffeeKinds);
		}

		[Route("send"), HttpPost]
		public async Task SendToTorrefacto()
		{
			if (!User.IsAdmin(_config))
				throw new UnauthorizedAccessException();

			var userOrders = (await _coffeeOrderRepository.GetAll())
				.SelectMany(_ => _.Packs)
				.GroupBy(_ => new { _.CoffeeKindName, _.Weight});

			var coffeeKinds = (await _coffeeKindRepository.GetAll())
				.OfType<AvailableCoffeeKind>()
				.ToDictionary(_ => _.Name);

			await _torrefactoClient.Authentificate();
			await _torrefactoClient.CleanupBasket();

			foreach (var order in userOrders)
			{
				AvailableCoffeeKind kind;
				if (!coffeeKinds.TryGetValue(order.Key.CoffeeKindName, out kind))
				{
					throw new InvalidOperationException("Coffee " + order.Key.CoffeeKindName + " is unavailable.");
				}

				await _torrefactoClient.AddToBasket(kind, order.First(), order.Count());
			}
		}

		[Route("clear"), HttpPost]
		public async Task ClearAllOrders()
		{
			if (!User.IsAdmin(_config))
				throw new UnauthorizedAccessException();

			await _coffeeOrderRepository.Clean();
		}

		private async Task<CoffeePack> getDesiredPack(string coffeeName, int weight)
		{
			var coffeeKind = await _coffeeKindRepository.Get(coffeeName);
			if (coffeeKind == null)
				throw new ArgumentException();

			var availableCoffeeKind = coffeeKind as AvailableCoffeeKind;
			if (availableCoffeeKind == null)
				throw new ArgumentException();

			var desiredPack = availableCoffeeKind
				.AvailablePacks
				.SingleOrDefault(p => p.Weight == weight);

			if (desiredPack == null)
				throw new ArgumentException();

			return desiredPack;
		}

		private readonly CoffeeKindRepository _coffeeKindRepository;
		private readonly CoffeeOrderRepository _coffeeOrderRepository;
		private readonly TorrefactoClient _torrefactoClient;
		private readonly Config _config;

		public class CoffeeKindModel
		{
			public string Name { get; set; }
			public CoffeePackModel[] Packs { get; set; }
			public bool IsAvailable { get; set; }
		}

		public class CoffeePackModel
		{
			public int Weight { get; set; }
			public int Price { get; set; }
			public int Count { get; set; }
		}
	}
}