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

        public Task<GroupCoffeeOrder?> GetCurrentOrder()
        {
            return Collection
                .Find(_ => _.IsActive)
                .SortByDescending(_ => _.CreatedAt)
                .FirstOrDefaultAsync()!;
        }

        public async Task Update(GroupCoffeeOrder o)
        {
            await Collection.ReplaceOneAsync(_ => _.Id == o.Id, o);
        }
    }
}