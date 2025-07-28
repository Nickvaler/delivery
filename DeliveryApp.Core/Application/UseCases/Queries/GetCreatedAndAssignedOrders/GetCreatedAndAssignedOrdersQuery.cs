using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    /// <summary>
    ///     Получить все незавершенные заказы (статус Created и Assigned)
    /// </summary>
    public class GetCreatedAndAssignedOrdersQuery : IRequest<GetCreatedAndAssignedOrdersQueryResponse>
    {
    }
}
