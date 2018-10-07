using System.Runtime.Serialization;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class RegistrationModel
	{
		[DataMember(Name = "token")]
		public string Token { get; set; }
		
		[DataMember(Name = "password")]
		public string Password { get; set; }
	}
}