using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface IRepository<T>
    {
        Task<IReadOnlyCollection<T>> GetAll();
        Task Clean();
        Task Insert(IReadOnlyCollection<T> elements);
    }
}