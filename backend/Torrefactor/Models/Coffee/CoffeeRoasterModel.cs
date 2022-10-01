namespace Torrefactor.Models.Coffee
{
    public class CoffeeRoasterModel
    {
        public string Id { get; private set; }
        public string Name { get; private set; }

        public CoffeeRoasterModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}