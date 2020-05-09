using System.Collections.Generic;
using Torrefactor.Core.Interfaces;

namespace Torrefactor.Core.Services
{
    public interface ICoffeeRoasterSelector
    {
        ICoffeeRoasterClient SelectFor(GroupCoffeeOrder order);
        IReadOnlyCollection<string> GetProviderIds();
    }
}