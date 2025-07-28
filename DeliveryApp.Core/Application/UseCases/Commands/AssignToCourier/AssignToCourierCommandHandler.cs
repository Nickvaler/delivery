using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using MediatR;
using Primitives;

namespace DeliveryApp.Core.Application.UseCases.Commands.AssignToCourier
{
    public class AssignToCourierCommandHandler(IUnitOfWork unitOfWork,
        IOrderRepository orderRepository,
        ICourierRepository courierRepository,
        IDispatchService dispatchService) : IRequestHandler<AssignToCourierCommand, UnitResult<Error>>
    {
        public async Task<UnitResult<Error>> Handle(AssignToCourierCommand request, CancellationToken cancellationToken)
        {
            var couriers = await courierRepository.GetAllAvailableAsync();
            var orderGetResult = await orderRepository.GetFirstInCreatedStatusAsync();
            var dispatchResult = dispatchService.Dispatch(orderGetResult.Value, couriers);
            if (dispatchResult.IsFailure)
            {
                return dispatchResult.Error;
            }
            orderGetResult.Value.Assign(dispatchResult.Value);
            courierRepository.Update(dispatchResult.Value);
            orderRepository.Update(orderGetResult.Value);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return UnitResult.Success<Error>();
        }
    }
}
