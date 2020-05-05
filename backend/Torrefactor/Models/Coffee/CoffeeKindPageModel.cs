using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Torrefactor.Models.Coffee
{
    public class CoffeeKindPageModel
    {
        [JsonProperty] public bool ActiveGroupOrderExists { get; private set; }
        
        [JsonProperty] public IReadOnlyCollection<CoffeeKindModel> CoffeeKinds { get; private set; }

        public CoffeeKindPageModel()
        {
            CoffeeKinds = new CoffeeKindModel[0];
        }

        public CoffeeKindPageModel(bool activeGroupOrderExists, IEnumerable<CoffeeKindModel> kinds)
        {
            ActiveGroupOrderExists = activeGroupOrderExists;
            CoffeeKinds = kinds.ToList();
        }
    }
}