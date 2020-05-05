using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public class CoffeeOrderService
    {
        private readonly ICoffeeKindRepository _coffeeKindRepository;
        private readonly ICoffeeProvider _coffeeProvider;
        private readonly IGroupCoffeeOrderRepository _groupCoffeeOrderRepository;

        public CoffeeOrderService(
            ICoffeeKindRepository coffeeKindRepository,
            IGroupCoffeeOrderRepository groupCoffeeOrderRepository,
            ICoffeeProvider coffeeProvider)
        {
            _coffeeKindRepository = coffeeKindRepository;
            _groupCoffeeOrderRepository = groupCoffeeOrderRepository;
            _coffeeProvider = coffeeProvider;
        }

        public async Task<GroupCoffeeOrder?> TryGetCurrentGroupOrder()
        {
            return await _groupCoffeeOrderRepository.GetCurrentOrder();
        }

        public async Task<PersonalCoffeeOrder?> TryGetPersonalOrder(string customerName)
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder == null)
                return null;

            return currentGroupOrder.TryGetPersonalOrder(customerName) ?? new PersonalCoffeeOrder(customerName);
        }

        public async Task CreateNewGroupOrder()
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder != null)
                throw new CoffeeOrderException("Can't create new order cause there is an active order");

            currentGroupOrder = new GroupCoffeeOrder();
            await _groupCoffeeOrderRepository.Insert(new[] {currentGroupOrder});
        }

        public async Task AddPackToOrder(string customerName, string coffeeName, int weight)
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder == null)
                throw new CoffeeOrderException("No group order available");

            var desiredPack = await GetDesiredPack(coffeeName, weight);
            currentGroupOrder.AddPack(customerName, desiredPack);

            await _groupCoffeeOrderRepository.Update(currentGroupOrder);
        }

        public async Task RemovePackFromOrder(string customerName, string coffeeName, int weight)
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder == null)
                throw new CoffeeOrderException("No group order available");

            var desiredPack = await GetDesiredPack(coffeeName, weight);
            currentGroupOrder.RemovePack(customerName, desiredPack);

            await _groupCoffeeOrderRepository.Update(currentGroupOrder);
        }

        public async Task SendToCoffeeProvider()
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder == null)
                throw new CoffeeOrderException("No group order available");

            try
            {
                currentGroupOrder.StartSending();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
                currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();

                await _coffeeProvider.Authenticate();
                await _coffeeProvider.CleanupBasket();

                await foreach (var order in GetCoffeePacksToSend(currentGroupOrder!))
                    await _coffeeProvider.AddToBasket(order.Kind, order.Pack, order.Count);

                currentGroupOrder!.MarkAsSent();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
            }
            catch (Exception)
            {
                currentGroupOrder!.MarkSendAsFailed();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
            }
        }

        private async IAsyncEnumerable<(AvailableCoffeeKind Kind, CoffeePack Pack, int Count)> GetCoffeePacksToSend(
            GroupCoffeeOrder order)
        {
            var coffeeKinds = (await _coffeeKindRepository.GetAll())
                .OfType<AvailableCoffeeKind>()
                .ToDictionary(_ => _.Name);

            var packsByCoffeeKind =
                from o in order.PersonalOrders
                from p in o.Packs
                group p by p.CoffeeKindName;

            foreach (var o in packsByCoffeeKind)
            {
                if (!coffeeKinds.TryGetValue(o.Key, out var kind))
                    throw new InvalidOperationException("Coffee " + o.Key + " is unavailable.");

                yield return (kind, o.First(), o.Count());
            }
        }

        private async Task<CoffeePack> GetDesiredPack(string coffeeName, int weight)
        {
            var coffeeKind = await _coffeeKindRepository.Get(coffeeName);
            if (coffeeKind == null)
                throw new ArgumentException($"Coffee kind {coffeeName} does not exist");

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
    }
}