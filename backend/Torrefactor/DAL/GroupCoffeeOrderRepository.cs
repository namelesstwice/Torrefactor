using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Models;

namespace Torrefactor.DAL
{
    public class GroupCoffeeOrderRepository : Repository<GroupCoffeeOrder>
    {
        public GroupCoffeeOrderRepository(IMongoDatabase db) : base(db, "groupCoffeeOrders")
        {
        }

        public Task<GroupCoffeeOrder> GetCurrentOrder()
        {
            return Collection
                .Find(_ => !_.IsSent)
                .SortByDescending(_ => _.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}