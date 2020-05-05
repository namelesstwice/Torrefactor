using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HtmlAgilityPack;
using Torrefactor.Core;

namespace Torrefactor.Infrastructure.CoffeeProviders.Torrefacto
{
    internal static class CoffeeListPageParser
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
                    let isNotAvailable = div.Descendants().Any(_ => HasClass(_, "button-disabled"))
                    select new CoffeeKind(name, isNotAvailable, packs)
                )
                .ToList();
        }

        private static IEnumerable<CoffeePack.Builder> TryParsePacks(HtmlNode div)
        {
            var priceHolder = div?.Descendants()
                .SingleOrDefault(_ => HasClass(_, "current-price"));

            if (priceHolder == null)
                yield break;

            var packSizeHolders = div?.Descendants()
                .Single(_ => HasClass(_, "offer-type"))
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

        private static bool HasClass(HtmlNode node, string className)
        {
            return node.Attributes.Any(a => a.Name == "class" && a.Value.Split(' ').Any(_ => _ == className));
        }

        private static bool HasAttribute(HtmlNode node, string name, string? value = null)
        {
            return node.Attributes.Any(a => a.Name == name && (value == null || a.Value == value));
        }
    }
}