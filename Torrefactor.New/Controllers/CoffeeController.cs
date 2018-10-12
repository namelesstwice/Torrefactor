using System;
using System.Collections;
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
	[Route("api/coffee")]
	public class CoffeeController : Controller
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

		[HttpGet("")]
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
					Packs = (kind as AvailableCoffeeKind)?.AvailablePacks
						.Select(pack => new CoffeePackModel
						{
							Price = pack.PriceWithRebate,
							Weight = pack.Weight,
							Count = userOrders.GetCount(kind, pack.Weight)
						})
						.ToArray(),
					IsAvailable = kind is AvailableCoffeeKind
				});
		}
		
		[HttpGet("orders")]
		public async Task<IEnumerable> GetAllOrders()
		{
			if (!User.IsAdmin(_config))
				throw new UnauthorizedAccessException();

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
						.Select(p => new CoffeeKindModel
						{
							Name = p.Key.CoffeeName,
							Packs = new []
							{
								new CoffeePackModel
								{
									Weight = p.Key.Weight,
									Count = p.Count()
								}
							}
						}),
					OverallState = o.Packs.Any(p => p.State == PackState.Unavailable)
						? PackState.Unavailable
						: o.Packs.Any(p => p.State == PackState.PriceChanged)
							? PackState.PriceChanged 
							: PackState.Available,
					Price = o.Packs.Sum(_ => _.PriceWithRebate)
				});
		}

		[HttpPost("add")]
		public async Task Add(string coffeeName, int weight)
		{
			var userOrders =
				(await _coffeeOrderRepository.GetUserOrders(User.Identity.Name))
				?? new CoffeeOrder(User.Identity.Name);

			var desiredPack = await getDesiredPack(coffeeName, weight);
			userOrders.AddCoffeePack(desiredPack);

			await _coffeeOrderRepository.Update(userOrders);
		}

		[HttpPost("remove")]
		public async Task Remove(string coffeeName, int weight)
		{
			var userOrders =
				(await _coffeeOrderRepository.GetUserOrders(User.Identity.Name))
				?? new CoffeeOrder(User.Identity.Name);

			var desiredPack = await getDesiredPack(coffeeName, weight);
			userOrders.RemoveCoffeePack(desiredPack);

			await _coffeeOrderRepository.Update(userOrders);
		}

		[HttpPost("reload")]
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

		[HttpPost("send")]
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
				if (!coffeeKinds.TryGetValue(order.Key.CoffeeKindName, out var kind))
				{
					throw new InvalidOperationException("Coffee " + order.Key.CoffeeKindName + " is unavailable.");
				}

				await _torrefactoClient.AddToBasket(kind, order.First(), order.Count());
			}
		}

		[HttpPost("clear")]
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