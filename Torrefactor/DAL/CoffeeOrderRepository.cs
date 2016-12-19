using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Exceptions;
using Torrefactor.Models;

namespace Torrefactor.DAL
{
	public class CoffeeOrderRepository : Repository<CoffeeOrder>
	{
		public CoffeeOrderRepository(IMongoDatabase db) : base(db, "coffeeOrders")
		{
			Collection.Indexes
				.CreateOneAsync(
					Builders<CoffeeOrder>.IndexKeys.Ascending(_ => _.Username), 
					new CreateIndexOptions { Unique = true })
				.Wait();
		}

		public async Task<CoffeeOrder> GetUserOrders(string userName)
		{
			return await Collection.Find(_ => _.Username == userName).SingleOrDefaultAsync();
		}

		public Task Update(CoffeeOrder userOrders, bool upsert = false)
		{
			var filter = Builders<CoffeeOrder>.Filter.Where(x => x.Username == userOrders.Username);
			Builders<CoffeeOrder>.Filter.And(filter, Builders<CoffeeOrder>.Filter.Where(x => x.ChangeStamp == userOrders.ChangeStamp));

			var updateResult = Collection.ReplaceOneAsync(filter, new CoffeeOrder(userOrders), new UpdateOptions { IsUpsert = upsert });
			if (updateResult == null)
				throw new ConcurrencyException("concurrency");

			return updateResult;
		}
	}
}