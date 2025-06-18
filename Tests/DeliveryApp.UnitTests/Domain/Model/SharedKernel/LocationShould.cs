using DeliveryApp.Core.Domain.Models.SharedKernel;
using FluentAssertions;
using Xunit;

namespace DeliveryApp.UnitTests.Domain.Model.SharedKernel
{
    public class LocationShould
    {
        [Fact]
        public void BeCorrectWhenParamsIsCorrectOnCreated()
        {
            //Arrange

            //Act
            var location = Location.Create(1, 2);

            //Assert
            location.Value.X.Should().Be(1);
            location.Value.Y.Should().Be(2);
        }

        [Theory]
        [InlineData(2, 2, 2, 2)]
        [InlineData(1, 1, 1, 1)]
        [InlineData(10, 10, 10, 10)]
        public void BeEquivalentWithSameParameters(byte testX, byte testY, byte resX, byte resY)
        {
            //Arrange
            var testLocation = Location.Create(testX, testY);

            //Act
            var resultLocation = Location.Create(resX, resY);

            //Assert
            Assert.Equal(testLocation, resultLocation);
        }

        [Theory]
        [InlineData(2, 6, 4, 9, 5)]
        [InlineData(2, 6, 4, 10, 6)]
        [InlineData(1, 1, 10, 10, 18)]
        [InlineData(1, 1, 1, 1, 0)]
        [InlineData(10, 10, 1, 1, 18)]
        [InlineData(8, 7, 4, 2, 9)]
        public void HarCorrectDistanceToLocation(
            byte toX,
            byte toY,
            byte curX,
            byte curY,
            int resultDistance)
        {
            //Arrange
            var toLocation = Location.Create(toX, toY);
            var currentLocation = Location.Create(curX, curY);

            //Act
            var distance = currentLocation.Value.CalculateDistance(toLocation.Value);

            //Assert
            distance.Should().Be(resultDistance);
        }


        [Fact]
        public void BeCorrectRandomLocation()
        {
            //Arrange

            //Act
            var location = Location.CreateRandom();

            //Assert
            Assert.True(location.Value.X >= 1);
            Assert.True(location.Value.X <= 10);
            Assert.True(location.Value.Y >= 1);
            Assert.True(location.Value.Y <= 10);
        }
    }
}
