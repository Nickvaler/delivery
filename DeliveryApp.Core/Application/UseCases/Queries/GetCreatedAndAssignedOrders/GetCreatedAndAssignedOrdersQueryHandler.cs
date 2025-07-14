using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders
{
    public class GetCreatedAndAssignedOrdersQueryHandler : IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersQueryResponse>
    {
        private readonly string _connectionString;

        public GetCreatedAndAssignedOrdersQueryHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }


        public async Task<GetCreatedAndAssignedOrdersQueryResponse> Handle(GetCreatedAndAssignedOrdersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var results = await connection.QueryAsync<GetCreatedAndAssignedOrdersQueryResponse.Order>(
                    @$"
                    SELECT 
                    id              AS {nameof(GetCreatedAndAssignedOrdersQueryResponse.Order.Id)}
                    ,courier_id     AS {nameof(GetCreatedAndAssignedOrdersQueryResponse.Order.CourierId)}
                    ,location_x     AS {nameof(GetCreatedAndAssignedOrdersQueryResponse.Order.LocationX)}
                    ,location_y     AS {nameof(GetCreatedAndAssignedOrdersQueryResponse.Order.LocationY)}
                    FROM public.orders
                    WHERE status != @status;"
                , new { status = OrderStatus.Completed.Name });

            return new GetCreatedAndAssignedOrdersQueryResponse(results.ToList());
        }
    }
}
