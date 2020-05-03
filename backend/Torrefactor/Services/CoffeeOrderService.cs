using System;
using System.Linq;
using System.Threading.Tasks;
using Torrefactor.DAL;
using Torrefactor.Models;

namespace Torrefactor.Services
{
    public class CoffeeOrderService
    {
	    private readonly CoffeeOrderRepository _coffeeOrderRepository;
	    private readonly GroupCoffeeOrderRepository _groupCoffeeOrderRepository;
	    private readonly CoffeeKindRepository _coffeeKindRepository;
	    private readonly ICoffeeProvider _coffeeProvider;

	    public CoffeeOrderService(
		    CoffeeOrderRepository coffeeOrderRepository, 
		    CoffeeKindRepository coffeeKindRepository, 
		    GroupCoffeeOrderRepository groupCoffeeOrderRepository, 
		    ICoffeeProvider coffeeProvider)
	    {
		    _coffeeOrderRepository = coffeeOrderRepository;
		    _coffeeKindRepository = coffeeKindRepository;
		    _groupCoffeeOrderRepository = groupCoffeeOrderRepository;
		    _coffeeProvider = coffeeProvider;
	    }

	    public async Task AddPackToOrder(string customerName, string coffeeName, int weight)
	    {
		    var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
		    if (currentGroupOrder == null)
			    throw new CoffeeOrderException("No group order available"); 
					
        	var userOrders =
        		(await _coffeeOrderRepository.GetUserOrders(customerName))
        		?? new CoffeeOrder(customerName, currentGroupOrder.Id);

        	var desiredPack = await getDesiredPack(coffeeName, weight);
        	userOrders.AddCoffeePack(desiredPack);

        	await _coffeeOrderRepository.Update(userOrders);
        }
	    
        public async Task RemovePackFromOrder(string customerName, string coffeeName, int weight)
        {
	        var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
	        if (currentGroupOrder == null)
		        throw new CoffeeOrderException("No group order available"); 
	        
        	var userOrders =
        		(await _coffeeOrderRepository.GetUserOrders(customerName))
        		?? new CoffeeOrder(customerName, currentGroupOrder.Id);

        	var desiredPack = await getDesiredPack(coffeeName, weight);
        	userOrders.RemoveCoffeePack(desiredPack);

        	await _coffeeOrderRepository.Update(userOrders);
        }
        
        public async Task SendToCoffeeProvider()
        {
	        var currentGroupOrder = await _groupCoffeeOrderRepository.GetCurrentOrder();
	        if (currentGroupOrder == null)
		        throw new CoffeeOrderException("No group order available"); 
	        
        	var userOrders = (await _coffeeOrderRepository.Get(currentGroupOrder.Id))
        		.SelectMany(_ => _.Packs)
        		.GroupBy(_ => new { _.CoffeeKindName, _.Weight});

        	var coffeeKinds = (await _coffeeKindRepository.GetAll())
        		.OfType<AvailableCoffeeKind>()
        		.ToDictionary(_ => _.Name);

        	await _coffeeProvider.Authenticate();
        	await _coffeeProvider.CleanupBasket();

        	foreach (var order in userOrders)
        	{
        		if (!coffeeKinds.TryGetValue(order.Key.CoffeeKindName, out var kind))
        		{
        			throw new InvalidOperationException("Coffee " + order.Key.CoffeeKindName + " is unavailable.");
        		}

        		await _coffeeProvider.AddToBasket(kind, order.First(), order.Count());
        	}
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
    }
}