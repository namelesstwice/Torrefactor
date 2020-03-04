using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using MongoDB.Driver;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Services;
using IdentityRole = Microsoft.AspNetCore.Identity.MongoDB.IdentityRole;
using IdentityUser = Microsoft.AspNetCore.Identity.MongoDB.IdentityUser;

namespace Torrefactor
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json");

            var config = builder.Build().Get<Config>();

            services.AddSingleton(config);
            services.AddSingleton(new MongoClient(config.MongodbConnectionString).GetDatabase(config.DatabaseName));
            services.AddSingleton<CoffeeKindRepository>();
            services.AddSingleton<CoffeeOrderRepository>();
            services.AddSingleton<InviteRepository>();
            services.AddSingleton<TorrefactoClient>();

            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .RegisterMongoStores(
                    provider => provider.GetService<IMongoDatabase>().GetCollection<IdentityUser>("users"),
                    provider => provider.GetService<IMongoDatabase>().GetCollection<IdentityRole>("roles"));

            services.Configure<IdentityOptions>(options =>
            {
                options.User.RequireUniqueEmail = true;
                
                options.Password.RequiredLength = 1;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.Password.RequireNonAlphanumeric = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.HttpOnly = true;
            });

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            services.AddSingleton<IFileProvider>(physicalProvider);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            env.WebRootFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            app.UseMvc(routes =>
            {
                routes.MapSpaFallbackRoute(
                    "spa-fallback",
                    new { controller = "Spa", action = "Index"});
            });


        }
    }
}