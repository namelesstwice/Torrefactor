using System.Runtime.Serialization;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class UserModel
	{
		[DataMember(Name = "id")]
		public string Id { get; }

		[DataMember(Name = "isAdmin")]
		public bool IsAdmin { get; }

		[DataMember(Name = "name")]
		public string? Name { get; }

		[DataMember(Name = "email")]
		public string Email { get; }

		public UserModel(bool isAdmin, string? displayName, string email, string id)
		{
			IsAdmin = isAdmin;
			Name = displayName;
			Email = email;
			Id = id;
		}
	}
}