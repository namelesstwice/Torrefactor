using Newtonsoft.Json;

namespace Torrefactor.Models.Coffee
{
    public class CoffeeRoasterModel
    {
        [JsonProperty] public string Id { get; private set; }
        [JsonProperty] public string Name { get; private set; }

        public CoffeeRoasterModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}