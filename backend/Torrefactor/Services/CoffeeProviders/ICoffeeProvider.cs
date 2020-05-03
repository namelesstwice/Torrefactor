using System.Collections.Generic;
using System.Threading.Tasks;
using Torrefactor.Models;

namespace Torrefactor.Services
{
    public interface ICoffeeProvider
    {
        Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds();
        Task Authenticate();
        Task CleanupBasket();
        Task AddToBasket(AvailableCoffeeKind kind, CoffeePack pack, int count);
    }
}