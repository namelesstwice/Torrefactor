using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface ICoffeeProvider
    {
        Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds();
        Task Authenticate();
        Task CleanupBasket();
        Task AddToBasket(CoffeeKind kind, CoffeePack pack, int count);
    }
}