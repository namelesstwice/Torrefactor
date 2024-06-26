﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Torrefactor.Infrastructure.DAL
{
    internal abstract class Repository<T>
    {
        protected readonly IMongoCollection<T> Collection;

        protected Repository(IMongoDatabase db, string collectionName)
        {
            Collection = db.GetCollection<T>(collectionName);
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