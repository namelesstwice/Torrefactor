using System.Collections.Generic;

namespace Torrefactor.Models.Coffee
{
    public class GroupCoffeeOrderModel
    {
        public bool HasActiveOrder { get; private set; }
        
        public IEnumerable<PersonalCoffeeOrderModel> PersonalOrders { get; }
        
        public string? RoasterId { get; private set; }


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