using System;
using System.Security.Cryptography;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models.Auth
{
	public class Invite
	{
		[BsonId]
		public ObjectId Id { get; private set; }
		
		[BsonElement("email"), BsonRequired]
		public string Email { get; private set; }
		
		[BsonElement("name"), BsonRequired]
		public string Name { get; private set; }
		
		[BsonElement("state"), BsonRequired]
		public InviteState State { get; private set; }
		
		[BsonElement("token"), BsonIgnoreIfNull]
		public string Token { get; private set; }
		
		[BsonElement("changeStamp"), BsonRequired]
		public Guid ChangeStamp { get; set; }

		public Invite(string email, string name)
		{
			Email = email;
			Name = name;
			State = InviteState.Pending;
			ChangeStamp = Guid.NewGuid();
		}

		public void Approve()
		{
			if (State != InviteState.Pending)
				throw new InvalidOperationException($"Can't approve invite {Id}, cause it is not pending");

			State = InviteState.Approved;
			 
			using (var rng = new RNGCryptoServiceProvider())
			{
				var tokenData = new byte[32];
				rng.GetBytes(tokenData);

				Token = Convert.ToBase64String(tokenData);
			}
		}

		public void Decline()
		{
			if (State != InviteState.Pending)
				throw new InvalidOperationException($"Can't approve invite {Id}, cause it is not pending");

			State = InviteState.Declined;
		}

		public void ResetToken()
		{
			if (State != InviteState.Approved)
				throw new InvalidOperationException($"Can't reset token on invite {Id}, cause it is not approved");

			Token = null;
		}
	}
}