using System.Collections.Generic;

namespace Torrefactor.Models
{
	public class Config
	{
		public Config()
		{
			AdminEmails = new List<string>();
		}

		public string TorrefactoLogin { get; set; }
		public string TorrefactoPassword { get; set; }
		public List<string> AdminEmails { get; set; }

		public string MongodbConnectionString { get; set; }
		public string DatabaseName { get; set; }
		public string Secret { get; set; }
	}
}