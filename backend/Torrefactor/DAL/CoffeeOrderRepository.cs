using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Torrefactor.Models;

namespace Torrefactor.DAL
{
	public class CoffeeOrderRepository : Repository<CoffeeOrder>
	{
		public CoffeeOrderRepository(IMongoDatabase db) : base(db, "coffeeOrders")
		{
		}

		public async Task<IReadOnlyCollection<CoffeeOrder>> Get(ObjectId groupOrderId)
		{
			return await Collection.Find(_ => _.GroupOrderId == groupOrderId).ToListAsync();
		}

		public async Task<CoffeeOrder> GetUserOrder(string userName, ObjectId groupOrderId)
		{
			return await Collection
				.Find(_ => _.Username == userName && _.GroupOrderId == groupOrderId)
				.SingleOrDefaultAsync();
		}

		public Task Update(CoffeeOrder userOrders)
		{
			return Collection.ReplaceOneAsync(_ => _.Username == userOrders.Username, userOrders, new ReplaceOptions { IsUpsert = true });
		}
	}
}