using System.Collections.Generic;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public interface ICoffeeProviderSelector
    {
        ICoffeeProvider SelectFor(GroupCoffeeOrder order);
        IReadOnlyCollection<string> GetProviderIds();
    }
}