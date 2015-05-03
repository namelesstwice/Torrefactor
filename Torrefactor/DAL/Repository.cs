using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Torrefactor.DAL
{
	public abstract class Repository<T>
	{
		protected readonly IMongoCollection<T> Collection;

		protected Repository(IMongoDatabase db, string collectionName)
		{
			Collection = db.GetCollection<T>(collectionName);
		}

		public async Task<IReadOnlyCollection<T>> Get(int start, int limit)
		{
			return await Collection.Find(el => true).Skip(start).Limit(limit).ToListAsync();
		}

		public async Task<IReadOnlyCollection<T>> GetAll()
		{
			return await Collection.Find(el => true).ToListAsync();
		}


		public Task Clean()
		{
			return Collection.DeleteManyAsync(el => true);
		}

		public Task Insert(IReadOnlyCollection<T> elements)
		{
			return Collection.InsertManyAsync(elements);
		}
	}
}