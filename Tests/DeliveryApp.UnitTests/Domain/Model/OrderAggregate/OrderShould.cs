using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.OrderAggregate;

public class OrderShould
{
    [Fact]
    public void BeCreatedSuccessfullyWithValidParameters()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var location = Location.Create(5, 10).Value;

        // Act
        var result = Order.Create(orderId, location, volume: 5);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(orderId);
        result.Value.Location.Should().Be(location);
        result.Value.Volume.Should().Be(5);
        result.Value.Status.Should().Be(OrderStatus.Created);
    }

    [Fact]
    public void FailToCreateWithEmptyGuid()
    {
        // Act
        var result = Order.Create(Guid.Empty, Location.Create(1, 1).Value, volume: 3);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("orderId");
    }

    [Fact]
    public void BeAssignedToCourier()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(1, 1).Value, 3).Value;
        var courier = Courier.Create("Ivan", 2, Location.Create(5, 4).Value).Value;

        // Act
        var result = order.Assign(courier);

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Assigned);
        order.CourierId.Should().Be(courier.Id);
    }

    [Fact]
    public void BeCompletedSuccessfullyIfAssigned()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(2, 3).Value, 4).Value;
        var courier = Courier.Create("Dima", 1, Location.Create(7, 9).Value).Value;
        order.Assign(courier);

        // Act
        var result = order.Complete();

        // Assert
        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public void FailToCompleteIfNotAssigned()
    {
        // Arrange
        var order = Order.Create(Guid.NewGuid(), Location.Create(2, 3).Value, 4).Value;

        // Act
        var result = order.Complete();

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Message.Should().Contain("Завершить можно только назначенный ранее заказ");
        order.Status.Should().Be(OrderStatus.Created);
    }
}
