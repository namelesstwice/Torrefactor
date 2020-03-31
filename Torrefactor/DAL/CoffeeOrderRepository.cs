using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Models;

namespace Torrefactor.DAL
{
	public class CoffeeOrderRepository : Repository<CoffeeOrder>
	{
		public CoffeeOrderRepository(IMongoDatabase db) : base(db, "coffeeOrders")
		{
		}

		public async Task<CoffeeOrder> GetUserOrders(string userName)
		{
			return await Collection.Find(_ => _.Username == userName).SingleOrDefaultAsync();
		}

		public Task Update(CoffeeOrder userOrders)
		{
			return Collection.ReplaceOneAsync(_ => _.Username == userOrders.Username, userOrders, new ReplaceOptions { IsUpsert = true });
		}
	}
}