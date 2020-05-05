using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Torrefactor.Core;

namespace Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee
{
    public static class RocketsCoffeeListPageParser
    {
        private static readonly Regex _whitespaces = new Regex(@"\s", RegexOptions.Compiled);
        private static readonly Regex _weight = new Regex(@"(?<num>\d+)\s+(?<suffix>.*)", RegexOptions.Compiled);

        public static IEnumerable<CoffeeKind> Parse(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            return
                from productNode in doc.DocumentNode.Descendants()
                where productNode.HasClass("catalog__product_back")
                let name = productNode.ChildNodes.Single(_ => _.HasClass("catalog__product_title")).InnerText.Trim()
                let packs = 
                    from @select in productNode.Descendants("select")
                    from option in @select.Descendants()
                    where @select.GetAttributeValue("for_size", "").Contains("в зёрнах") &&
                          option.HasAttribute("data-price")
                    let priceRaw = option.GetAttributeValue("data-price", null)
                    let weightRaw = option.GetAttributeValue("value", null)
                    select CoffeePack.Create(ParseWeight(weightRaw), ParsePrice(priceRaw)).SetId("123")
                select new CoffeeKind(name, true, packs);
        }

        private static int ParsePrice(string? priceRaw) 
            => int.Parse(_whitespaces.Replace(priceRaw, ""));

        private static int ParseWeight(string? weightRaw)
        {
            var match = _weight.Match(weightRaw);
            if (!match.Success)
                throw new FormatException($"Invalid pack weight {weightRaw}");
            
            var suffix = match.Groups["suffix"].Value;
            var number = match.Groups["num"].Value;
            
            return suffix switch
            {
                "г" => int.Parse(number),
                "кг" => int.Parse(number) * 1000,
                _ => throw new FormatException($"Unknown unit {suffix}")
            };
        }

        private static bool HasAttribute(this HtmlNode node, string name, string? value = null) 
            => node.Attributes.Any(a => a.Name == name && (value == null || a.Value == value));
    }
}