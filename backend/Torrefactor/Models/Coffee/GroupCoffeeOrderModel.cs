using System.Collections.Generic;
using Newtonsoft.Json;

namespace Torrefactor.Models.Coffee
{
    public class GroupCoffeeOrderModel
    {
        [JsonProperty] public bool HasActiveOrder { get; private set; }
        
        [JsonProperty] public IEnumerable<PersonalCoffeeOrderModel> PersonalOrders { get; }

        public GroupCoffeeOrderModel(bool hasActiveOrder, IEnumerable<PersonalCoffeeOrderModel>? personalOrders = null)
        {
            HasActiveOrder = hasActiveOrder;
            PersonalOrders = personalOrders ?? new PersonalCoffeeOrderModel[0];
        }
    }
}