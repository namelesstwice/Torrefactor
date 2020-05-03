using System.Linq;
using System.Security.Principal;
using Torrefactor.Models;

namespace Torrefactor.Services
{
	public static class AuthExtensions
	{
		public static bool IsAdminEmail(string? email, Config cfg) => cfg.AdminEmails.Any(_ => _ == email);

		public static bool IsAdmin(this IPrincipal principal) => principal.IsInRole("admin");
	}
}