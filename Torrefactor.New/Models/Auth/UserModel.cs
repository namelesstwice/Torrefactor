using System.Runtime.Serialization;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class UserModel
	{
		[DataMember(Name = "isAdmin")]
		public bool IsAdmin { get; }
		
		[DataMember(Name = "name")]
		public string Name { get; }

		public UserModel(bool isAdmin, string name)
		{
			IsAdmin = isAdmin;
			Name = name;
		}
	}
}