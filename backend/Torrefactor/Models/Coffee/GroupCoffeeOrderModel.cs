using System.Collections.Generic;
using Newtonsoft.Json;

namespace Torrefactor.Models.Coffee
{
    public class GroupCoffeeOrderModel
    {
        [JsonProperty] public bool HasActiveOrder { get; private set; }
        
        [JsonProperty] public IEnumerable<PersonalCoffeeOrderModel> PersonalOrders { get; }
        
        [JsonProperty] public string? RoasterId { get; private set; }


        public GroupCoffeeOrderModel(
            bool hasActiveOrder, 
            string? roasterId = null,
            IEnumerable<PersonalCoffeeOrderModel>? personalOrders = null)
        {
            HasActiveOrder = hasActiveOrder;
            RoasterId = roasterId;
            PersonalOrders = personalOrders ?? new PersonalCoffeeOrderModel[0];
        }
    }
}