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
        private readonly IGroupCoffeeOrderRepository _groupCoffeeOrderRepository;
        private readonly ICoffeeProviderSelector _coffeeProviderSelector;

        public CoffeeOrderService(
            ICoffeeKindRepository coffeeKindRepository,
            IGroupCoffeeOrderRepository groupCoffeeOrderRepository,
            ICoffeeProviderSelector coffeeProviderSelector)
        {
            _coffeeKindRepository = coffeeKindRepository;
            _groupCoffeeOrderRepository = groupCoffeeOrderRepository;
            _coffeeProviderSelector = coffeeProviderSelector;
        }

        public async Task<GroupCoffeeOrder?> TryGetCurrentGroupOrder()
        {
            var currentOrder =  await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentOrder == null)
                return null;
            
            var kinds = (await _coffeeKindRepository.GetAll()).ToDictionary(p => p.Name);

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

            return currentOrder;
        }

        public async Task<(PersonalCoffeeOrder? Order, bool ActiveGroupOrderExists)> TryGetPersonalOrder(string customerName)
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            return currentGroupOrder switch
            {
                null => (null, false),
                _ => (currentGroupOrder.TryGetPersonalOrder(customerName), true)
            };
        }

        public async Task CreateNewGroupOrder(string providerId)
        {
            var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
            if (currentGroupOrder != null)
                throw new CoffeeOrderException("Can't create new order cause there is an active order");

            currentGroupOrder = new GroupCoffeeOrder(providerId);
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

            var coffeeProvider = _coffeeProviderSelector.SelectFor(currentGroupOrder);
            
            try
            {
                currentGroupOrder.StartSending();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
                currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();

                await coffeeProvider.Authenticate();
                await coffeeProvider.CleanupBasket();

                await foreach (var order in GetCoffeePacksToSend(currentGroupOrder!))
                    await coffeeProvider.AddToBasket(order.Kind, order.Pack, order.Count);

                currentGroupOrder!.MarkAsSent();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
            }
            catch (Exception)
            {
                currentGroupOrder!.MarkSendAsFailed();
                await _groupCoffeeOrderRepository.Update(currentGroupOrder);
            }
        }

        private async IAsyncEnumerable<(CoffeeKind Kind, CoffeePack Pack, int Count)> GetCoffeePacksToSend(GroupCoffeeOrder order)
        {
            var coffeeKinds = (await _coffeeKindRepository.GetAll())
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
            
            if (!coffeeKind.IsAvailable)
                throw new ArgumentException();

            var desiredPack = coffeeKind
                .AvailablePacks
                .SingleOrDefault(p => p.Weight == weight);

            if (desiredPack == null)
                throw new ArgumentException();

            return desiredPack;
        }
    }
}