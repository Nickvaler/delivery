using static DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders.GetCreatedAndAssignedOrdersQueryResponse;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderQueryService
    {
        Task<IEnumerable<OrderDto>> GetCreatedAndAssignedOrdersAsync(CancellationToken cancellationToken);
    }
}
