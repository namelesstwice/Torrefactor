using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Owin;
using Microsoft.Owin.Extensions;
using MongoDB.Driver;
using NConfiguration;
using NConfiguration.Combination;
using NConfiguration.GenericView;
using NConfiguration.Joining;
using NConfiguration.Xml;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Web.Common.OwinHost;
using Ninject.Web.WebApi.OwinHost;
using Owin;
using Torrefactor.DAL;
using Torrefactor.Models;
using Torrefactor.Services;

[assembly: OwinStartup(typeof(Torrefactor.Startup))]

namespace Torrefactor
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			// For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888

			var webCfg = new HttpConfiguration();
			webCfg.Formatters.Clear();
			webCfg.Formatters.Add(new JsonMediaTypeFormatter
			{
				SerializerSettings = new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				}
			});
			webCfg.MapHttpAttributeRoutes();

			app.UseNinjectMiddleware(register);
			app.UseNinjectWebApi(webCfg);

			RegisterMvcRoutes(RouteTable.Routes);
		}

		public static void RegisterMvcRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{action}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}

		private static ICombinableAppSettings loadSettings()
		{
			var strConv = new StringConverter();
			var deserializer = new GenericDeserializer();

			var xmlFileLoader = new XmlFileSettingsLoader(deserializer, strConv);
			var loader = new SettingsLoader(xmlFileLoader);

			loader.LoadSettings(new XmlSystemSettings("ExtConfigure", strConv, deserializer));

			var result = loader.Settings;
			return new CombinableAppSettings(result);
		}

		private static IKernel register()
		{
			var settings = loadSettings();
			var kernel = new StandardKernel();

			// Cfg
			kernel.Bind<Config>().ToMethod(ctx => settings.Get<Config>());

			// DAL
			kernel.Bind<IMongoDatabase>().ToConstant(new MongoClient("mongodb://localhost:27017").GetDatabase("coffee"));
			kernel.Bind<CoffeeKindRepository>().ToSelf().InSingletonScope();
			kernel.Bind<CoffeeOrderRepository>().ToSelf().InSingletonScope();

			// SVC
			kernel.Bind<TorrefactoClient>().ToSelf().InSingletonScope();

			return kernel;
		}
	}
}
