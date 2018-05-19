using System.Collections.Generic;

namespace Torrefactor.Models
{
	public class Config
	{
		public string TorrefactoLogin { get; set; }
		public string TorrefactoPassword { get; set; }
		public List<string> AdminNames { get; set; }

		public string MongodbConnectionString { get; set; }
		public string DatabaseName { get; set; }
	}
}