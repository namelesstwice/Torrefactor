using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Torrefactor.Core.Interfaces;
using Torrefactor.Core.Services;
using Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee;
using Torrefactor.Infrastructure.CoffeeProviders.Torrefacto;
using Torrefactor.Infrastructure.DAL;

namespace Torrefactor.Infrastructure
{
    public static class InfrastructureModule
    {
        public static void Load(IServiceCollection services)
        {
            services.AddSingleton(p =>
            {
                var config = p.GetService<Config>();
                return new MongoClient(config.MongodbConnectionString).GetDatabase(config.DatabaseName);
            });

            services.AddSingleton<ICoffeeKindRepository, CoffeeKindRepository>();
            services.AddSingleton<IGroupCoffeeOrderRepository, GroupCoffeeOrderRepository>();
            services.AddSingleton<ICoffeeRoasterClient, TorrefactoClient>();
            services.AddSingleton<ICoffeeRoasterClient, RocketsCoffeeClient>();
        }
    }
}