using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Torrefactor.Models
{
    public class GroupCoffeeOrder
    {
        [BsonId]
        public ObjectId Id { get; private set; }

        [BsonElement("createdAt"), BsonRequired]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("isSent"), BsonRequired] 
        public bool IsActive => State != GroupCoffeeOrderState.Sent && State != GroupCoffeeOrderState.Canceled;
        
        [BsonElement("state"), BsonRequired]
        public GroupCoffeeOrderState State { get; private set; }

        [BsonIgnore]
        public IReadOnlyCollection<PersonalCoffeeOrder> PersonalOrders => _personalOrders.Values;
        
        [BsonElement("personalOrders"), BsonRequired]
        private readonly Dictionary<string, PersonalCoffeeOrder> _personalOrders;

        public GroupCoffeeOrder()
        {
            CreatedAt = DateTime.UtcNow;
            _personalOrders = new Dictionary<string, PersonalCoffeeOrder>();
        }

        public PersonalCoffeeOrder? TryGetPersonalOrder(string customerName)
        {
            _personalOrders.TryGetValue(customerName, out var order);
            return order;
        }

        public void AddPack(string customerName, CoffeePack pack)
        {
            if (State != GroupCoffeeOrderState.Created && State != GroupCoffeeOrderState.SendFailed)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            if (!_personalOrders.TryGetValue(customerName, out var personalOrder))
            {
                personalOrder = new PersonalCoffeeOrder(customerName);
                _personalOrders[customerName] = personalOrder;
            }
            
            personalOrder.AddCoffeePack(pack);
        }

        public void RemovePack(string customerName, CoffeePack pack)
        {
            if (State != GroupCoffeeOrderState.Created && State != GroupCoffeeOrderState.SendFailed)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            if (!_personalOrders.TryGetValue(customerName, out var personalOrder))
            {
                personalOrder = new PersonalCoffeeOrder(customerName);
                _personalOrders[customerName] = personalOrder;
            }
            
            personalOrder.RemoveCoffeePack(pack);
        }

        public void StartSending()
        {
            if (State != GroupCoffeeOrderState.Created && State != GroupCoffeeOrderState.SendFailed)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            State = GroupCoffeeOrderState.Sending;
        }

        public void MarkAsSent()
        {
            if (State != GroupCoffeeOrderState.Sending)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            State = GroupCoffeeOrderState.Sent;
        }
        
        public void MarkSendAsFailed()
        {
            if (State != GroupCoffeeOrderState.Sending)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            State = GroupCoffeeOrderState.SendFailed;
        }

        public void Cancel()
        {
            if (State == GroupCoffeeOrderState.Sent || State == GroupCoffeeOrderState.Canceled)
                throw new InvalidOperationException($"Invalid state: {State}; Order id: {Id}");

            State = GroupCoffeeOrderState.Canceled;
        }
    }

    public enum GroupCoffeeOrderState
    {
        Created,
        Sending,
        Sent,
        SendFailed,
        Canceled
    }
}