using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Torrefactor.Models;

namespace Torrefactor.Services
{
	public class CoffeeListPageParser
	{
		public static IReadOnlyCollection<CoffeeKind> Parse(Stream stream)
		{
			var htmlDoc = new HtmlDocument();
			htmlDoc.Load(stream, Encoding.UTF8);

			return (
				from div in htmlDoc.DocumentNode.Descendants("div")
				where hasAttribute(div, "data-entity", "item")
				let torrefactoId = int.Parse(div.Attributes["data-item-id"].Value)
				let packs = tryParsePacks(div)
				let mainName = div.Descendants().Single(_ => hasClass(_, "card-title")).InnerText
				let suffix = div.Descendants().SingleOrDefault(_ => hasClass(_, "text-caps"))?.InnerText
				let name = $"{mainName} {suffix}".Trim()
				let isNotAvailable = div.Descendants().Any(_ => hasClass(_, "button-disabled"))
				select isNotAvailable
					? new CoffeeKind(name)
					: new AvailableCoffeeKind(name, torrefactoId, packs)
				)
				.ToList();
		}

		private static IEnumerable<CoffeePack.Builder> tryParsePacks(HtmlNode div)
		{
			return div?.Descendants()
				.Single(_ => hasClass(_, "card-info"))
				.Descendants()
				.Where(_ => hasClass(_, "flex-auto"))
				.Select(p =>
				{
					var weightHolder = p.ChildNodes.Single(_ => hasClass(_, "mb-5"));
					var weight = int.Parse(weightHolder.InnerText.Trim('г', ' '));

					var priceHolder = p.Descendants("input").Single();
					var id = priceHolder.Attributes["value"].Value;
					var price = int.Parse(priceHolder.Attributes["data-price"].Value);

					return CoffeePack.Create(weight, price).SetId(id);
				});
		}

		private static bool hasClass(HtmlNode node, string className)
		{
			return node.Attributes.Any(a => a.Name == "class" && a.Value.Split(' ').Any(_ => _ == className));
		}

		private static bool hasAttribute(HtmlNode node, string name, string value)
		{
			return node.Attributes.Any(a => a.Name == name && a.Value == value);
		}
	}
}