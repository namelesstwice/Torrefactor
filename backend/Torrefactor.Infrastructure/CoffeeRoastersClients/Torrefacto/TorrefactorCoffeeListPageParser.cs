using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Torrefactor.Core;

namespace Torrefactor.Infrastructure.CoffeeProviders.Torrefacto
{
    internal static class TorrefactorCoffeeListPageParser
    {
        public static IReadOnlyCollection<CoffeeKind> Parse(Stream stream)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.Load(stream, Encoding.UTF8);

            return (
                    from div in htmlDoc.DocumentNode.Descendants("div")
                    where HasAttribute(div, "data-entity", "item")
                    let packs = TryParsePacks(div)
                    let name = div.Descendants()
                        .Single(_ => HasAttribute(_, "data-prop-name"))
                        .Attributes["data-prop-name"].Value
                    let isAvailable = !div.Descendants().Any(_ => _.HasClass("button-disabled"))
                    select new CoffeeKind(name, isAvailable, packs)
                )
                .ToList();
        }

        private static IEnumerable<CoffeePack.Builder> TryParsePacks(HtmlNode div)
        {
            var priceHolder = div?.Descendants()
                .SingleOrDefault(_ => _.HasClass("current-price"));

            if (priceHolder == null)
                yield break;

            var packSizeHolders = div?.Descendants()
                .Single(_ => _.HasClass("offer-type"))
                .Descendants("option");

            if (packSizeHolders == null)
                yield break;

            foreach (var packSizeHolder in packSizeHolders)
            {
                var price = int.Parse(priceHolder.InnerText.Trim(' ', '₽'));
                var packSize = int.Parse(packSizeHolder.InnerText.Replace('г', ' ').Trim());
                var id = packSizeHolder.Attributes["value"].Value;

                yield return CoffeePack.Create(packSize, price).SetId(id);
            }
        }

        private static bool HasAttribute(HtmlNode node, string name, string? value = null)
        {
            return node.Attributes.Any(a => a.Name == name && (value == null || a.Value == value));
        }
    }
}