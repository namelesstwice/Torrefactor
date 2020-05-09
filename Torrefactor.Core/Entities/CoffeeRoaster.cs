using System;

namespace Torrefactor.Core
{
    public class CoffeeRoaster
    {
        public string Id { get; }
        
        public string Name { get; }

        public CoffeeRoaster(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}