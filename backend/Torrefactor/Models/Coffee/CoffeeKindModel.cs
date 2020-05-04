namespace Torrefactor.Controllers
{
    public class CoffeeKindModel
    {
        public string Name { get; private set; }
        public CoffeePackModel[] Packs { get; private set; }
        public bool IsAvailable { get; private set; }
        public CoffeePackModel? SmallPack { get; private set; }
        public CoffeePackModel? BigPack { get; private set; }

        public CoffeeKindModel(
            string name, 
            CoffeePackModel[]? packs, 
            bool isAvailable, 
            CoffeePackModel? smallPack,
            CoffeePackModel? bigPack)
        {
            Name = name;
            Packs = packs ?? new CoffeePackModel[0];
            IsAvailable = isAvailable;
            SmallPack = smallPack;
            BigPack = bigPack;
        }
    }
}