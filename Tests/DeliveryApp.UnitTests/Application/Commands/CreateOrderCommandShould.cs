using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using FluentAssertions;
using NSubstitute;
using Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DeliveryApp.UnitTests.Application.Commands
{
    public class CreateOrderCommandShould
    {
        private readonly IOrderRepository _orderRepositoryMock;
        private readonly IUnitOfWork _unitOfWork;

        public CreateOrderCommandShould()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _orderRepositoryMock = Substitute.For<IOrderRepository>();
        }

        [Fact]
        public async Task ReturnTrueWhenOrderCreatedSuccessfully()
        {
            //Arrange
            _orderRepositoryMock.GetAsync(Arg.Any<Guid>())
                .Returns(Order.Create(Guid.NewGuid(), Location.CreateRandom().Value, 3).Value);
            _unitOfWork.SaveChangesAsync()
                .Returns(Task.FromResult(true));

            //Act
            var createCreateOrderCommandResult = CreateOrderCommand.Create(Guid.NewGuid(), "улица");
            createCreateOrderCommandResult.IsSuccess.Should().BeTrue();

            var handler = new CreateOrderCommandHandler(_orderRepositoryMock, _unitOfWork);
            var result = await handler.Handle(createCreateOrderCommandResult.Value, new CancellationToken());

            //Assert
            _orderRepositoryMock.Received(1);
            _unitOfWork.Received(1);
            result.IsSuccess.Should().BeTrue();
        }

    }
}
