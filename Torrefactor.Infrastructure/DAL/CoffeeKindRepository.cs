using System.Threading.Tasks;
using MongoDB.Driver;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Infrastructure.DAL
{
    internal sealed class CoffeeKindRepository : Repository<CoffeeKind>, ICoffeeKindRepository
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