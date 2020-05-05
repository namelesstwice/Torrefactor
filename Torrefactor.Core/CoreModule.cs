using Microsoft.Extensions.DependencyInjection;
using Torrefactor.Core.Services;

namespace Torrefactor.Core
{
    public static class CoreModule
    {
        public static void Load(IServiceCollection services)
        {
            services.AddSingleton<CoffeeOrderService>();
            services.AddSingleton<CoffeeKindService>();
            services.AddSingleton<ICoffeeProviderSelector, CoffeeProviderSelector>();
        }
    }
}