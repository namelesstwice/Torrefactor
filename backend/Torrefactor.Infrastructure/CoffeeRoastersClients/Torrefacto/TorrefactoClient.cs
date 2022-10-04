using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Threading.Tasks;
using Torrefactor.Core;
using Torrefactor.Core.Interfaces;
using Torrefactor.Infrastructure.Utils;

namespace Torrefactor.Infrastructure.CoffeeProviders.Torrefacto
{
    internal sealed class TorrefactoClient : ICoffeeRoasterClient
    {
        private const string _torrefactoHostName = "https://www.torrefacto.ru";
        private readonly CookieAwareWebClient _client = new CookieAwareWebClient();
        private readonly Config _config;

        public TorrefactoClient(Config config)
        {
            _config = config;
        }
        
        public CoffeeRoaster Roaster { get; } = new CoffeeRoaster("TF", "Torrefacto");

        public async Task<IReadOnlyCollection<CoffeeKind>> GetCoffeeKinds()
        {
            var request = WebRequest.CreateHttp(new Uri(getFullUrl("catalog/roasted/")));
            var response = await request.GetResponseAsync();
            return TorrefactorCoffeeListPageParser.Parse(response.GetResponseStream());
        }

        public async Task Authenticate(string key)
        {
            await _client.DownloadStringTaskAsync(getFullUrl(""));
            await _client.UploadValuesTaskAsync(getFullUrl("ajaxa.php"), new NameValueCollection
            {
                {"USER_LOGIN", _config.TorrefactoLogin},
                {"USER_PASSWORD", _config.TorrefactoPassword}
            });
        }

        public Task CleanupBasket()
        {
            return _client.UploadStringTaskAsync(getFullUrl("include/bas_delete.php"), "POST", "");
        }

        public Task AddToBasket(CoffeeKind kind, CoffeePack pack, int count)
        {
            var torrefactoId = kind.GetActualExternalId(pack);
            var data = new NameValueCollection
            {
                {"action", "ADD2BASKET"},
                {"ajax_buy", "1"},
                {"typ", "0"},
                {"id_0", torrefactoId},
                {"quantity", count.ToString(CultureInfo.InvariantCulture)},
                {"HOW_0", "Не молоть"},
                {"package", "Зип-лок"}
            };

            return _client.UploadValuesTaskAsync(getFullUrl("/ajax.php"), data);
        }

        private static string getFullUrl(string relativePath)
        {
            return $"{_torrefactoHostName}/{relativePath}";
        }
    }
}