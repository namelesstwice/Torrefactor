using System;

namespace Torrefactor.Services
{
    public class CoffeeOrderException : Exception
    {
        public CoffeeOrderException(string message) : base(message)
        {
        }
    }
}