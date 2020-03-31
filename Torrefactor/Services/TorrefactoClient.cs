using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Torrefactor.Models;
using Torrefactor.Utils;

namespace Torrefactor.Services
{
	public class TorrefactoClient
	{
		public TorrefactoClient(Config config)
		{
			_config = config;
		}

		public async Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds()
		{
			var request = WebRequest.CreateHttp(new Uri(getFullUrl("catalog/roasted/")));
			var response = await request.GetResponseAsync();
			return CoffeeListPageParser.Parse(response.GetResponseStream());
		}

		public async Task Authenticate()
		{
			await _client.DownloadStringTaskAsync(getFullUrl(""));
			await _client.UploadValuesTaskAsync(getFullUrl("ajaxa.php"), new NameValueCollection
			{
				{"USER_LOGIN", _config.TorrefactoLogin},
				{"USER_PASSWORD", _config.TorrefactoPassword},
			});
		}

		public Task CleanupBasket()
		{
			return _client.UploadStringTaskAsync(getFullUrl("include/bas_delete.php"), "POST", "");
		}

		public Task AddToBasket(AvailableCoffeeKind kind, CoffeePack pack, int count)
		{
			var torrefactoId = kind.GetActualTorrefactoId(pack);
			var data = new NameValueCollection
			{
				{"action", "ADD2BASKET"}, 
				{"ajax_buy", "1"},
				{"typ", "0"},
				{"id_0", torrefactoId},
				{"quantity", count.ToString(CultureInfo.InvariantCulture)},
				{"HOW_0", "Не молоть"},
				{"package", "Зип-лок" }
			};

			return _client.UploadValuesTaskAsync(getFullUrl("/ajax.php"), data);
		}

		private static string getFullUrl(string relativePath)
		{
			return $"{_torrefactoHostName}/{relativePath}";
		}

		private const string _torrefactoHostName = "https://www.torrefacto.ru";
		private readonly CookieAwareWebClient _client = new CookieAwareWebClient();
		private readonly Config _config;
	}
}