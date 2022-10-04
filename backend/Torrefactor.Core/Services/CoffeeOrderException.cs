using System;

namespace Torrefactor.Core.Services
{
    public class CoffeeOrderException : Exception
    {
        public CoffeeOrderException(string message) : base(message)
        {
        }
    }
}