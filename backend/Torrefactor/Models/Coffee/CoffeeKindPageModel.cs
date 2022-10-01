using System.Collections.Generic;
using System.Linq;

namespace Torrefactor.Models.Coffee
{
    public class CoffeeKindPageModel
    {
        public bool ActiveGroupOrderExists { get; private set; }
        
        public IReadOnlyCollection<CoffeeKindModel> CoffeeKinds { get; private set; }

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