using Newtonsoft.Json;

namespace Torrefactor.Models.Coffee
{
    public class CoffeeKindModel
    {
        public CoffeeKindModel()
        {
        }

        public CoffeeKindModel(
            string name,
            CoffeePackModel[]? packs,
            bool isAvailable,
            CoffeePackModel? smallPack = null,
            CoffeePackModel? bigPack = null)
        {
            Name = name;
            Packs = packs ?? new CoffeePackModel[0];
            IsAvailable = isAvailable;
            SmallPack = smallPack;
            BigPack = bigPack;
        }

        [JsonProperty] public string Name { get; private set; } = "";

        [JsonProperty] public CoffeePackModel[] Packs { get; private set; } = new CoffeePackModel[0];

        [JsonProperty] public bool IsAvailable { get; private set; }

        [JsonProperty] public CoffeePackModel? SmallPack { get; private set; }

        [JsonProperty] public CoffeePackModel? BigPack { get; private set; }
    }
}