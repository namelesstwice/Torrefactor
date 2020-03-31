using System.Linq;
using System.Security.Principal;
using Torrefactor.Models;

namespace Torrefactor.Services
{
	public static class AuthExtensions
	{
		public static bool IsAdmin(this IPrincipal principal, Config cfg)
		{
			return cfg.AdminNames.Any(_ => _ == principal.Identity.Name);
		}
	}
}