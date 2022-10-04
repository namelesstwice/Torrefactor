using System.Collections.Generic;

namespace Torrefactor.Infrastructure
{
    public class Config
    {
        public string TorrefactoLogin { get; set; } = "";
        public string TorrefactoPassword { get; set; } = "";
        public List<string> AdminEmails { get; set; } = new List<string>();

        public string MongodbConnectionString { get; set; } = "";
        public string DatabaseName { get; set; } = "";
        public string Secret { get; set; } = "";
    }
}