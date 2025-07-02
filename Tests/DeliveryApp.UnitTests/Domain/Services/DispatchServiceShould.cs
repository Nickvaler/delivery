using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Domain.Services;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Services
{
    public class DispatchServiceShould
    {
        [Fact]
        public void ShouldReturnCourierAfterCorrectDispatch()
        {
            //  Arrange
            var dispatchService = new DispatchService();
            var order = Order.Create(Guid.NewGuid(), Location.Create(8, 8).Value, 12).Value;

            var courier1 = Courier.Create("Иван", 2, Location.Create(2, 5).Value).Value;
            courier1.AddStoragePlace("Вторая сумка", 20);

            var courier2 = Courier.Create("Петр", 5, Location.Create(4, 8).Value).Value;
            courier2.AddStoragePlace("Чемодан сумка", 15);

            var couriers = new List<Courier>()
            {
                courier1,
                courier2,
                Courier.Create("Петр", 5, Location.Create(4,8).Value).Value,
                Courier.Create("Николай", 1, Location.Create(1,1).Value).Value,
                Courier.Create("Дима", 3, Location.Create(9,2).Value).Value,
            };

            //  Act
            var dispatchResult = dispatchService.Dispatch(order, couriers);

            //  Assert
            dispatchResult.Value.Should().NotBeNull();
        }
    }
}
