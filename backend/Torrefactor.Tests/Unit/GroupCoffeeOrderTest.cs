using System;
using System.Linq;
using NUnit.Framework;
using Torrefactor.Core;

namespace Torrefactor.Tests.Unit
{
    [TestFixture]
    public class GroupCoffeeOrderTest
    {
        [Test]
        public void Should_not_allow_to_add_or_remove_pack_when_sending_in_progress([Values(true, false)] bool isAdd)
        {
            var order = new GroupCoffeeOrder();
            order.StartSending();
            
            Assert.Throws<InvalidOperationException>(() => 
                AddOrRemove(order, isAdd));
        }
        
        [Test]
        public void Should_not_allow_to_add_or_remove_pack_when_sending_is_completed([Values(true, false)] bool isAdd)
        {
            var order = new GroupCoffeeOrder();
            order.StartSending();
            order.MarkAsSent();
            
            Assert.Throws<InvalidOperationException>(() => 
                AddOrRemove(order, isAdd));
        }
        
        [Test]
        public void Should_not_allow_to_add_or_remove_pack_when_order_is_canceled([Values(true, false)] bool isAdd)
        {
            var order = new GroupCoffeeOrder();
            order.Cancel();
            
            Assert.Throws<InvalidOperationException>(() => 
                AddOrRemove(order, isAdd));
        }

        [Test]
        public void Should_allow_to_add_pack_when_order_is_created_or_send_failed([Values(true, false)] bool isSendFailed)
        {
            var order = new GroupCoffeeOrder();

            if (isSendFailed)
            {
                order.StartSending();
                order.MarkSendAsFailed();
            }
            
            AddOrRemove(order, true);
            
            Assert.AreEqual(1, order.TryGetPersonalOrder("John Doe")!.Packs.Count);
        }
        
        [Test]
        public void Should_allow_to_remove_pack_when_order_is_created_or_send_failed([Values(true, false)] bool isSendFailed)
        {
            var order = new GroupCoffeeOrder();

            if (isSendFailed)
            {
                order.StartSending();
                order.MarkSendAsFailed();
            }
            
            AddOrRemove(order, true);
            AddOrRemove(order, false);
            
            Assert.AreEqual(0, order.TryGetPersonalOrder("John Doe")!.Packs.Count);
        }

        [Test]
        [TestCase(GroupCoffeeOrderState.Canceled)]
        [TestCase(GroupCoffeeOrderState.Sending)]
        [TestCase(GroupCoffeeOrderState.SendFailed)]
        public void Should_not_allow_to_change_state_of_sent_order(GroupCoffeeOrderState targetState)
        {
            var order = new GroupCoffeeOrder();
            order.StartSending();
            order.MarkAsSent();

            Assert.Throws<InvalidOperationException>(() => 
            {
                switch (targetState)
                {
                    case GroupCoffeeOrderState.Sending:
                        order.StartSending();
                        break;
                    case GroupCoffeeOrderState.SendFailed:
                        order.MarkSendAsFailed();
                        break;
                    case GroupCoffeeOrderState.Canceled:
                        order.Cancel();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(targetState), targetState, null);
                }
            });
        }

        [Test]
        public void Should_group_packs_by_coffee_kind_and_weight_when_counting_unique_packs_count()
        {
            var order = new GroupCoffeeOrder();
            
            AddOrRemove(order, true);
            AddOrRemove(order, true);
            
            CollectionAssert.AreEqual(
                new [] { 2 }, 
                order
                        .TryGetPersonalOrder("John Doe")!
                        .GetUniquePacksCount()
                        .Select(_ => _.Count));
        }

        private void AddOrRemove(GroupCoffeeOrder order, bool isAdd)
        {
            if (isAdd)
            {
                order.AddPack("John Doe", _testCoffeeKind.AvailablePacks.Single());
            }
            else
            {
                order.RemovePack("John Doe", _testCoffeeKind.AvailablePacks.Single());
            }
        }

        private readonly CoffeeKind _testCoffeeKind = new CoffeeKind("123", true, new[]
        {
            CoffeePack.Create(123, 123).SetId("123")
        });
    }
}