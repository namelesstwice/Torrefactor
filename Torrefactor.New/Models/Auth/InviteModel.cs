using System.Runtime.Serialization;

namespace Torrefactor.Models.Auth
{
	[DataContract]
	public class InviteModel
	{
		[DataMember(Name = "email")]
		public string Email { get; set; }
		
		[DataMember(Name = "name")]
		public string Name { get; set; }
	}
}