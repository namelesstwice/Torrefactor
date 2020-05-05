using System.Linq;
using System.Security.Principal;
using Torrefactor.Infrastructure;

namespace Torrefactor.Utils
{
    public static class AuthExtensions
    {
        public static bool IsAdminEmail(string? email, Config cfg)
        {
            return cfg.AdminEmails.Any(_ => _ == email);
        }

        public static bool IsAdmin(this IPrincipal principal)
        {
            return principal.IsInRole("admin");
        }
    }
}