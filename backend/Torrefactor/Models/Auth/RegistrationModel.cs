using System.Runtime.Serialization;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class RegistrationModel
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }
		
		[DataMember(Name = "token")]
		public string Email { get; set; }
		
		[DataMember(Name = "password")]
		public string Password { get; set; }
	}
}