using System;

namespace Torrefactor.Exceptions
{
	public class ConcurrencyException : Exception
	{
		public ConcurrencyException(string message) : base(message)
		{
		}
	}
}