using CSharpFunctionalExtensions;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.MoveCourier
{
    public class MoveCourierCommandHandler(
        IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        ICourierRepository courierRepository) : IRequestHandler<MoveCourierCommand, UnitResult<Error>>
    {
        public async Task<UnitResult<Error>> Handle(MoveCourierCommand request, CancellationToken cancellationToken)
        {
            var assignedOrders = await orderRepository.GetAllAssignedAsync();
            if (assignedOrders.Count == 0)
            {
                return UnitResult.Success<Error>();
            }

            foreach (var order in assignedOrders)
            {
                if (order.CourierId == null)
                {
                    return GeneralErrors.ValueIsInvalid(nameof(order.CourierId));
                }

                var courier = await courierRepository.GetAsync((Guid)order.CourierId);

                var courierMoveResult = courier.Move(order.Location);
                if (courierMoveResult.IsFailure) return courierMoveResult;

                if (order.Location == courier.Location)
                {
                    order.Complete();
                    courier.FinishOrder(order);
                }

                courierRepository.Update(courier);
                orderRepository.Update(order);
            }

            await unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success<Error>();
        }
    }
}
