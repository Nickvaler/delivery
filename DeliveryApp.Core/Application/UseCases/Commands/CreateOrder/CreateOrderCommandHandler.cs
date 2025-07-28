using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.CreateOrder
{
    public class CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IUnitOfWork unitOfWork) : IRequestHandler<CreateOrderCommand, UnitResult<Error>>
    {
        public async Task<UnitResult<Error>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var location = Location.CreateRandom();
            var orderVolume = 5;
            var orderCreateResult = Order.Create(request.BasketId, location.Value, orderVolume);
            if (orderCreateResult.IsFailure)
            {
                return orderCreateResult.Error;
            }
            await orderRepository.AddAsync(orderCreateResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
