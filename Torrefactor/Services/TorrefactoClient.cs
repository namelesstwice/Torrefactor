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
using Torrefactor.Models;
using Torrefactor.Utils;
using JObject = Newtonsoft.Json.Linq.JObject;

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

			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(response.GetResponseStream(), Encoding.UTF8);

			var coffeeKinds = 
				from div in htmlDoc.DocumentNode.Descendants("div")
				where hasClass(div, "morh")
				let priceHolder = div.ParentNode
					.ChildNodes.SingleOrDefault(n => hasClass(n, "price-hold"))
				let torrefactoId = tryParseId(priceHolder)
				let packsAndPrices = tryParsePacksAndPrices(priceHolder)
				let name = getName(div.ChildNodes.Single(n => n.Name == "div" && hasClass(n, "h3")))
				select new
				{
					Name = name,
					TorrefactoId = torrefactoId,
					PacksAndPrices = packsAndPrices?.ToArray()
				};

			var result = new List<CoffeeKind>();

			foreach (var kind in coffeeKinds)
			{
				if (kind.TorrefactoId == null)
				{
					result.Add(new CoffeeKind(kind.Name));
					continue;
				}

				applyPackIds(kind.PacksAndPrices, (await getPackIds(kind.TorrefactoId)).ToArray());

				result.Add(new AvailableCoffeeKind(kind.Name, int.Parse(kind.TorrefactoId), kind.PacksAndPrices));
			}

			return result;
		}

		private string getName(HtmlNode headerDiv)
		{
			var name = headerDiv.ChildNodes.Single(n => n.Name == "span").InnerText.Trim();
			var regionEtc = headerDiv.ChildNodes.FirstOrDefault(n => n.Name == "p")?.InnerText.Trim();

			return !string.IsNullOrWhiteSpace(regionEtc)
				? $"{name} ({regionEtc})"
				: name;
		}

		private void applyPackIds(CoffeePack.Builder[] packsAndPrices, string[] packIds)
		{
			if (packIds.Length != packsAndPrices.Length)
				throw new InvalidOperationException("Parsed pack ids count doesn't match parsed packs count.");

			if (!tryMatchPacksByWeights(packsAndPrices, packIds) &&
			    !tryMatchPacksByPrices(packsAndPrices, packIds))
			{
				throw new InvalidOperationException("Invalid pack ids: can't parse weight or price.");
			}
		}

		private static bool tryMatchPacksByWeights(CoffeePack.Builder[] packsAndPrices, string[] packIds)
		{
			return tryMatchPacks(packsAndPrices, packIds, id => id.Split('-').Last(), p => p.Weight.ToString(CultureInfo.InvariantCulture));
		}

		private static bool tryMatchPacksByPrices(CoffeePack.Builder[] packsAndPrices, string[] packIds)
		{
			return tryMatchPacks(packsAndPrices, packIds, id => id.Split('-').Skip(1).First(), p => p.Price.ToString(CultureInfo.InvariantCulture));
		}

		private static bool tryMatchPacks(
			CoffeePack.Builder[] packsAndPrices, 
			string[] packIds, 
			Func<string, string> attributeParser,
			Func<CoffeePack.Builder, string> attributeSelector)
		{
			HashSet<CoffeePack.Builder> matchedPacks = new HashSet<CoffeePack.Builder>();

			var parsedAttributes = packIds.Select(attributeParser).ToArray();
			for (int i = 0; i < parsedAttributes.Length; i++)
			{
				var matchingPack = packsAndPrices.FirstOrDefault(p => attributeSelector(p) == parsedAttributes[i]);
				if (matchingPack == null)
					return false;

				matchingPack.SetId(packIds[i]);
				matchedPacks.Add(matchingPack);
			}

			return matchedPacks.Count == packsAndPrices.Length;
		}

		public async Task Authentificate()
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

		private static async Task<IEnumerable<string>> getPackIds(string coffeKindId)
		{
			var request = WebRequest.CreateHttp(
				new Uri(getFullUrl($"include/ajax.basket.php?id={coffeKindId}&type=roasted&action=getProductPopup")));
			var responseString = await request.GetResponseStirngAsync();

			var html = JObject.Parse(responseString).Value<string>("response");
			using (var stringReader = new StringReader(html))
			{
				var htmlDoc = new HtmlDocument();
				htmlDoc.Load(stringReader);

				return htmlDoc.DocumentNode.Descendants("select")
					.Single(_ => hasName(_, "roasted-data"))
					.Descendants("option")
					.Select(n => n.Attributes["value"].Value);
			}
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
			return priceHolder
				?.ChildNodes.SingleOrDefault(n => n.Name == "a")
				?.Attributes.SingleOrDefault(a => a.Name == "data-id")
				?.Value;
		}

		private static bool hasClass(HtmlNode node, string className)
		{
			return node.Attributes.Any(a => a.Name == "class" && a.Value == className);
		}

		private static bool hasName(HtmlNode node, string name)
		{
			return node.Attributes.Any(a => a.Name == "name" && a.Value == name);
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