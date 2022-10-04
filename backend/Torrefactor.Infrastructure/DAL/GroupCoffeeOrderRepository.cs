using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Infrastructure.DAL
{
    internal sealed class GroupCoffeeOrderRepository : Repository<GroupCoffeeOrder>, IGroupCoffeeOrderRepository
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