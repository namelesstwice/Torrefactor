using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface ICoffeeKindRepository
    {
        Task<CoffeeKind> Get(string name);
        
        Task<IReadOnlyCollection<CoffeeKind>> GetAll();
        
        Task Clean();
        
        Task Insert(IReadOnlyCollection<CoffeeKind> elements);
    }
}