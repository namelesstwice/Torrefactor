using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface IGroupCoffeeOrderRepository : IRepository<GroupCoffeeOrder>
    {
        Task<GroupCoffeeOrder?> GetCurrentOrder();
        Task Update(GroupCoffeeOrder o);
    }
}