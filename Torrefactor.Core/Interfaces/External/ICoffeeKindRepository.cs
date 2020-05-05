using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface ICoffeeKindRepository : IRepository<CoffeeKind>
    {
        Task<CoffeeKind> Get(string name);
    }
}