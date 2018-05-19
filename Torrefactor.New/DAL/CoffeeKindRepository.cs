using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Models;

namespace Torrefactor.DAL
{
	public class CoffeeKindRepository : Repository<CoffeeKind>
	{
		public CoffeeKindRepository(IMongoDatabase db) : base(db, "coffeeKinds")
		{
		}

		public Task<CoffeeKind> Get(string name)
		{
			return Collection.Find(_ => _.Name == name).SingleOrDefaultAsync();
		}
	}
}