using System.Runtime.Serialization;
using MongoDB.Bson;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class SignInModel
	{
		[DataMember(Name = "email")]
		public string Email { get; set; }
		
		[DataMember(Name = "password")]
		public string Password { get; set; }
	}
}