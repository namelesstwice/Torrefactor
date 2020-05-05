using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Torrefactor.Infrastructure.CoffeeProviders.RocketsCoffee;

namespace Torrefactor.Tests.Unit
{
    [TestFixture]
    public class RocketsCoffeeParseTest
    {
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Torrefactor.Tests.Data.rockets.coffee.html";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            using var reader = new StreamReader(stream);
            _coffeeListPage = reader.ReadToEnd();
        }

        [Test]
        public void Should_parse_coffee_kinds()
        {
            var coffeeKinds = RocketsCoffeeListPageParser.Parse(_coffeeListPage).ToArray();

            foreach (var kind in coffeeKinds)
            {
                TestContext.WriteLine(kind.Name);

                foreach (var pack in kind.AvailablePacks)
                {
                    TestContext.WriteLine($"\tPrice: {pack.Price}, Price per 100 g: {pack.PricePer100g}, Weight: {pack.Weight}");
                }
            }
            
            Assert.That(coffeeKinds.Count(), Is.GreaterThan(0));
        }

        private string _coffeeListPage;
    }
}