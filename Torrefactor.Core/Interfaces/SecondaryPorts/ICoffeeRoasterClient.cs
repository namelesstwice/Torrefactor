using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface ICoffeeRoasterClient
    {
        CoffeeRoaster Roaster { get; }
        Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds();
        Task Authenticate(string key);
        Task CleanupBasket();
        Task AddToBasket(CoffeeKind kind, CoffeePack pack, int count);
    }
}