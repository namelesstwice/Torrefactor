using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Torrefactor.Models;

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
			var request = WebRequest.CreateHttp(new Uri("http://www.torrefacto.ru/catalog/roasted/"));
			var response = await request.GetResponseAsync();

			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);

			var coffeeKinds = 
				from div in htmlDoc.DocumentNode.Descendants("div")
				where hasClass(div, "morh")
				let priceHolder = div.ParentNode
					.ChildNodes.SingleOrDefault(n => hasClass(n, "price-hold"))
				let torrefactoId = tryParseId(priceHolder)
				let packsAndPrices = tryParsePacksAndPrices(priceHolder)
				let name = div.ChildNodes.Single(n => n.Name == "h3").InnerText.Trim()
				select new
				{
					Name = name,
					TorrefactoId = torrefactoId,
					PacksAndPrices = packsAndPrices != null ? packsAndPrices.ToArray() : null
				};

			var result = new List<CoffeeKind>();

			foreach (var kind in coffeeKinds)
			{
				if (kind.TorrefactoId == null)
				{
					result.Add(new CoffeeKind(kind.Name));
					continue;
				}

				var options = await getPackOptions(kind.TorrefactoId);
				foreach (var pack in kind.PacksAndPrices)
				{
					pack.SetId(options[pack.Weight.ToString(CultureInfo.InvariantCulture)]);
				}

				result.Add(new AvailableCoffeeKind(kind.Name, int.Parse(kind.TorrefactoId), kind.PacksAndPrices));
			}

			return result;
		}

		public async Task Authentificate()
		{
			await _client.DownloadStringTaskAsync("http://torrefacto.ru");
			await _client.UploadValuesTaskAsync("http://torrefacto.ru/ajaxa.php", new NameValueCollection
			{
				{"USER_LOGIN", _config.TorrefactoLogin},
				{"USER_PASSWORD", _config.TorrefactoPassword},
			});
		}

		public Task CleanupBasket()
		{
			return _client.UploadStringTaskAsync("http://torrefacto.ru/include/bas_delete.php", "POST", "");
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
			};

			return _client.UploadValuesTaskAsync("http://torrefacto.ru/ajax.php", data);
		}

		private static async Task<Dictionary<string, string>> getPackOptions(string id)
		{
			var request = WebRequest.CreateHttp(
				new Uri(String.Format((string)"http://www.torrefacto.ru/ajax.php?id={0}&type=roasted", id)));
			var response = await request.GetResponseAsync();

			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);

			var options = htmlDoc.DocumentNode.Descendants("select")
				.Single(_ => hasName(_, "id_0"))
				.Descendants("option")
				.Select(n => n.Attributes["value"].Value)
				.ToDictionary(_ => _.Split('-').Last());

			return options;
		}

		private static IEnumerable<CoffeePack.Builder> tryParsePacksAndPrices(HtmlNode priceHolder)
		{
			if (priceHolder == null)
				return null;

			return priceHolder
				.ChildNodes.Single(n => hasClass(n, "price-block"))
				.ChildNodes.Where(n => hasClass(n, "row"))
				.Select(n => CoffeePack.Create(
				
					weight: int.Parse(
						n.ChildNodes.Single(_ => hasClass(_, "weight")).InnerText.Replace(" г", "")),

					price: int.Parse(
						n.ChildNodes.Single(_ => hasClass(_, "price")).InnerText.Replace("&nbsp;&#8399;", ""))
				));
		}

		private static string tryParseId(HtmlNode priceHolder)
		{
			if (priceHolder == null)
				return null;

			return priceHolder
				.ChildNodes.Single(n => n.Name == "a")
				.Attributes.Single(a => a.Name == "data-id")
				.Value;
		}

		private static bool hasClass(HtmlNode node, string className)
		{
			return node.Attributes.Any(a => a.Name == "class" && a.Value == className);
		}

		private static bool hasName(HtmlNode node, string name)
		{
			return node.Attributes.Any(a => a.Name == "name" && a.Value == name);
		}

		private readonly CookieAwareWebClient _client = new CookieAwareWebClient();
		private readonly Config _config;
	}
}