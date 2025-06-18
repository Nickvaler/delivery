using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.CourierAggregate
{
    public class CourierShould
    {
        public static IEnumerable<object[]> GetCourierTestData()
        {
            yield return new object[] { "Ivan", 2, Location.Create(1, 1).Value };
            yield return new object[] { "Igor", 1, Location.Create(2, 3).Value };
            yield return new object[] { "Sasha", 5, Location.Create(9, 9).Value };
        }

        public static IEnumerable<object[]> GetCourierTestDataWithTargetLocation()
        {
            yield return new object[] { "Ivan", 2, Location.Create(1, 1).Value, Location.Create(2, 8).Value, 4, Location.Create(2, 2).Value };
            yield return new object[] { "Igor", 1, Location.Create(2, 3).Value, Location.Create(1, 10).Value, 8, Location.Create(1, 3).Value };
            yield return new object[] { "Sasha", 3, Location.Create(9, 9).Value, Location.Create(2, 7).Value, 3, Location.Create(6, 9).Value };
            yield return new object[] { "Dima", 10, Location.Create(5, 5).Value, Location.Create(4, 10).Value, 1, Location.Create(4, 10).Value };
        }

        [Theory]
        [MemberData(nameof(GetCourierTestData), MemberType = typeof(CourierShould))]
        public void BeCorrectWhenParamsIsCorrectOnCreated(string name, int speed, Location location)
        {
            //Arrange

            //Act
            var courier = Courier.Create(name, speed, location);

            //Assert
            courier.IsSuccess.Should().BeTrue();
            courier.Value.Id.Should().NotBeEmpty();
            courier.Value.Name.Should().Be(name);
            courier.Value.Speed.Should().Be(speed);
            courier.Value.Location.Should().NotBeNull();
            courier.Value.Location.X.Should().Be(location.X);
            courier.Value.Location.Y.Should().Be(location.Y);
            courier.Value.StoragePlaces.Count.Should().Be(1);
        }

        [Theory]
        [MemberData(nameof(GetCourierTestData), MemberType = typeof(CourierShould))]
        public void BeWithTwoStoragePlacesAfterAddOne(string name, int speed, Location location)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);
            var storagePlace = StoragePlace.Create("Trunk", 15);

            //Act
            courier.Value.AddStoragePlace(storagePlace.Value.Name, storagePlace.Value.TotalVolume);

            //Assert
            courier.Value.StoragePlaces.Count.Should().Be(2);
        }

        [Theory]
        [MemberData(nameof(GetCourierTestData), MemberType = typeof(CourierShould))]
        public void CanTakeVolumeForOrder(string name, int speed, Location location)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);
            var volume = 5;

            //Act
            var result = courier.Value.CanTakeVolumeForOrder(volume);

            //Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeTrue();
        }

        [Theory]
        [MemberData(nameof(GetCourierTestData), MemberType = typeof(CourierShould))]
        public void BeWithOrderAfterTakeOrder(string name, int speed, Location location)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);
            var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value, 5);

            //Act
            var result = courier.Value.TakeOrder(order.Value);

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Value.StoragePlaces.Find(sp => sp.OrderId == order.Value.Id).Should().NotBeNull();
        }

        [Theory]
        [MemberData(nameof(GetCourierTestData), MemberType = typeof(CourierShould))]
        public void BeCorrectAfterFinishOrder(string name, int speed, Location location)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);
            var order = Order.Create(Guid.NewGuid(), Location.CreateRandom().Value, 5);

            //Act
            courier.Value.TakeOrder(order.Value);
            var result = courier.Value.FinishOrder(order.Value);

            //Assert
            result.IsSuccess.Should().BeTrue();
            courier.Value.StoragePlaces.Count.Should().Be(1);
            courier.Value.StoragePlaces[0].OrderId.Should().BeNull();
        }

        [Theory]
        [MemberData(nameof(GetCourierTestDataWithTargetLocation), MemberType = typeof(CourierShould))]
        public void ReturnCorrectStepsCountToTarget(string name, int speed, Location location, Location target, int correctSteps, Location intermediateLocation)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);

            //Act
            var steps = courier.Value.CalculateStepsToLocation(target);

            //Assert
            steps.IsSuccess.Should().BeTrue();
            steps.Value.Should().Be(correctSteps);
        }

        [Theory]
        [MemberData(nameof(GetCourierTestDataWithTargetLocation), MemberType = typeof(CourierShould))]
        public void BeInCorrectLocationAfterMove(string name, int speed, Location location, Location target, int correctSteps, Location intermediateLocation)
        {
            //Arrange
            var courier = Courier.Create(name, speed, location);

            //Act
            var result = courier.Value.Move(target);

            //Assert
            courier.Value.Location.Should().Be(intermediateLocation);
        }
    }
}
