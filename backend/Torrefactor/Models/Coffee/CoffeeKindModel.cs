
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

        public string Name { get; private set; } = "";

        public CoffeePackModel[] Packs { get; private set; } = new CoffeePackModel[0];

        public bool IsAvailable { get; private set; }

        public CoffeePackModel? SmallPack { get; private set; }

        public CoffeePackModel? BigPack { get; private set; }
    }
}