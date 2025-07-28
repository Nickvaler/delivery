using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    public class GetCreatedAndAssignedOrdersQueryHandler(IOrderQueryService orderQueryService) : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersQueryResponse>
    {
        public async Task<GetCreatedAndAssignedOrdersQueryResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken cancellationToken)
        {
            var results = await orderQueryService.GetCreatedAndAssignedOrdersAsync(cancellationToken);

            return new GetCreatedAndAssignedOrdersQueryResponse(results);
        }
    }
}
