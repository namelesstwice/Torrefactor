using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Torrefactor.Core;
using Torrefactor.Infrastructure.CoffeeProviders.Torrefacto;

namespace Torrefactor.Tests.Unit;

[TestFixture]
public class TorrefactoParseTest
{
    private IReadOnlyCollection<CoffeeKind> _coffeekinds;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "Torrefactor.Tests.Data.torrefacto.html";

        var coffeeListPage = assembly.GetManifestResourceStream(resourceName);
        _coffeekinds = TorrefactorCoffeeListPageParser.Parse(coffeeListPage);
    }
    
    [Test]
    public void Should_parse_coffee_kinds()
    {
        CollectionAssert.IsNotEmpty(_coffeekinds);
    }

    [Test]
    public void Every_kind_should_have_at_least_one_pack()
    {
        foreach (var kind in _coffeekinds)
        {
            TestContext.WriteLine(kind.Name);
            CollectionAssert.IsNotEmpty(kind.AvailablePacks);
        }
    }
}