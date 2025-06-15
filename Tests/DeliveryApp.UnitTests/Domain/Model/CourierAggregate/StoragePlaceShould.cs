using DeliveryApp.Core.Domain.Models.CourierAggregate;
using FluentAssertions;
using System;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class StoragePlaceShould
    {
        [Theory]
        [InlineData("Bag", 20)]
        [InlineData("Trunk", 50)]
        public void BeCorrectWhenParamsIsCorrectOnCreated(string name, int totalVolume)
        {
            //Arrange

            //Act
            var result = StoragePlace.Create(name, totalVolume);

            //Assert
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Name.Should().Be(name);
            result.Value.TotalVolume.Should().Be(totalVolume);
        }

        [Theory]
        [InlineData("", 10)]
        [InlineData("Trunk", 0)]
        [InlineData("Bag", -1)]
        [InlineData("", -1)]
        public void BeFailedWhenParamsIsInCorrectOnCreated(string name, int totalVolume)
        {
            //Arrange

            //Act
            var result = StoragePlace.Create(name, totalVolume);
            //Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeDifferentWhenIdsAreNotEqual()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 10);
            var trunkPlace = new StoragePlace("Trunk", 20);

            // Act & Assert
            Assert.NotEqual(bagPlace, trunkPlace);
            Assert.False(bagPlace == trunkPlace);
            Assert.True(bagPlace != trunkPlace);
        }

        [Fact]
        public void BeEmptyOnCreate()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 10);

            // Act

            var result = bagPlace.IsEmpty();

            // Assert
            result.Should().Be("да");
        }

        [Fact]
        public void BeWithCorrectOrderAfterSetOrder()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 55);
            var orderGuid = Guid.NewGuid();
            var orderVolume = 30;

            // Act
            bagPlace.SetOrder(orderGuid, orderVolume);
            var result = bagPlace.IsEmpty();

            // Assert
            result.Should().Be("нет");
            bagPlace.Id.Should().NotBeEmpty();
            bagPlace.OrderId.Should().NotBeEmpty();
        }

        [Fact]
        public void BeFailedAfterSetEmptyOrder()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 55);
            var orderGuid = Guid.Empty;
            var orderVolume = 30;

            // Act
            var result = bagPlace.SetOrder(orderGuid, orderVolume);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeFailedAfterSetOrderWithNotEmptyOrder()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 55);
            var orderId = Guid.NewGuid();
            var orderVolume = 30;

            var newOrderId = Guid.NewGuid();
            var newOrderVolume = 40;

            // Act
            bagPlace.SetOrder(orderId, orderVolume);
            var result = bagPlace.SetOrder(newOrderId, newOrderVolume);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeFailedAfterSetVolumeMoreThanTotal()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 34);
            var orderId = Guid.NewGuid();
            var orderVolume = 49;

            // Act
            var result = bagPlace.SetOrder(orderId, orderVolume);

            // Assert
            result.IsFailure.Should().BeTrue();
        }

        [Fact]
        public void BeWithEmptyOrderAfterExtract()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 10);
            var orderGuid = Guid.NewGuid();
            var orderVolume = 5;

            // Act
            bagPlace.SetOrder(orderGuid, orderVolume);
            bagPlace.Extract();
            var result = bagPlace.IsEmpty();

            // Assert
            result.Should().Be("да");
        }

        [Fact]
        public void BeFailedWhenExtractPlaceWithEmptyOrder()
        {
            // Arrange
            var bagPlace = new StoragePlace("Bag", 10);

            // Act
            var result = bagPlace.Extract();

            // Assert
            result.IsFailure.Should().BeTrue();
        }
    }
}
