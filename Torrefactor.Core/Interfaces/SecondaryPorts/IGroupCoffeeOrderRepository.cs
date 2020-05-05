using System.Collections.Generic;
using System.Threading.Tasks;

namespace Torrefactor.Core.Interfaces
{
    public interface IGroupCoffeeOrderRepository
    {
        Task<GroupCoffeeOrder?> GetCurrentOrder();
        Task Update(GroupCoffeeOrder o);
        Task Insert(IReadOnlyCollection<GroupCoffeeOrder> groupCoffeeOrders);
    }
}