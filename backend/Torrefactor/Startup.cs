using System.IO;
using System.Text;
using AspNetCore.Identity.Mongo;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Models.Auth;
using Torrefactor.Services;

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
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.development.json", true);

            services.AddSingleton(p => builder.Build().Get<Config>());
            services.AddSingleton(p =>
            {
                var config = p.GetService<Config>();
                return new MongoClient(config.MongodbConnectionString).GetDatabase(config.DatabaseName);
            });
            services.AddSingleton<CoffeeKindRepository>();
            services.AddSingleton<CoffeeOrderRepository>();
            services.AddSingleton<TorrefactoClient>();
            
            services.AddIdentityMongoDbProvider<ApplicationUser, ApplicationRole>(identityOptions =>
            {
                identityOptions.User.RequireUniqueEmail = true;
                identityOptions.Password.RequiredLength = 1;
                identityOptions.Password.RequireLowercase = false;
                identityOptions.Password.RequireUppercase = false;
                identityOptions.Password.RequireNonAlphanumeric = false;
                identityOptions.Password.RequireDigit = false;
                identityOptions.Password.RequiredUniqueChars = 0;
                identityOptions.SignIn.RequireConfirmedAccount = true;
            }, mongoIdentityOptions => {
                var sp = services.BuildServiceProvider();
                var cfg = sp.GetService<Config>();
                mongoIdentityOptions.ConnectionString = $"{cfg.MongodbConnectionString}/{cfg.DatabaseName}";
                mongoIdentityOptions.UsersCollection = "users";
            });
            
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                var sp = services.BuildServiceProvider();
                var cfg = sp.GetService<Config>();
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(cfg.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            IFileProvider physicalProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());

            services.AddSingleton<IFileProvider>(physicalProvider);
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //app.UseHttpsRedirection();
            }
            
            app.UseStaticFiles();
            env.WebRootFileProvider = new PhysicalFileProvider(Directory.GetCurrentDirectory());
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                //endpoints.MapFallbackToController("Index", "Spa");
            });
        }
    }
}