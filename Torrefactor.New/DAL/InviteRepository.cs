using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Torrefactor.Models.Auth;

namespace Torrefactor.DAL
{
	public class InviteRepository : Repository<Invite>
	{
		public InviteRepository(IMongoDatabase db) : base(db, "invites")
		{
			Collection.Indexes.CreateOne(new CreateIndexModel<Invite>(
				Builders<Invite>.IndexKeys.Ascending(i => i.Email),
				new CreateIndexOptions
				{
					Name = "IX_Email",
					Unique = true
				}));
			
			Collection.Indexes.CreateOne(new CreateIndexModel<Invite>(
				Builders<Invite>.IndexKeys.Ascending(i => i.Token),
				new CreateIndexOptions
				{
					Name = "IX_Token",
					Unique = true,
					Sparse = true
				}));
			
			Collection.Indexes.CreateOne(new CreateIndexModel<Invite>(
				Builders<Invite>.IndexKeys.Ascending(i => i.State),
				new CreateIndexOptions
				{
					Name = "IX_State"
				}));
		}

		public async Task<Invite> Get(ObjectId id)
		{
			return await Collection.Find(i => i.Id == id).SingleOrDefaultAsync();
		}
		
		public async Task<Invite> Get(string token)
		{
			return await Collection.Find(i => i.Token == token).SingleOrDefaultAsync();
		}
		
		public async Task<IReadOnlyCollection<Invite>> GetInState(InviteState state)
		{
			return await Collection.Find(i => i.State == state).ToListAsync();
		}

		public async Task Update(Invite invite)
		{
			var result = await Collection.UpdateOneAsync(
				i => i.Id == invite.Id && i.ChangeStamp == invite.ChangeStamp,
				Builders<Invite>.Update
					.Set(i => i.State, invite.State)
					.Set(i => i.Token, invite.Token)
					.Set(i => i.ChangeStamp, Guid.NewGuid()));
			
			if (result.ModifiedCount == 0)
				throw new ApplicationException($"Can't update invite with id: {invite.Id}");
		}
	}
}