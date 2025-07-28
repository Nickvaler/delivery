using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Npgsql;
using static DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders.GetCreatedAndAssignedOrdersQueryResponse;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries
{
    public class OrderQueryService(IOptions<Settings> settings) : IOrderQueryService
    {
        public async Task<IEnumerable<OrderDto>> GetCreatedAndAssignedOrdersAsync(CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(settings.Value.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            var query = @$"
                    SELECT 
                    id              AS {nameof(OrderDto.Id)}
                    ,courier_id     AS {nameof(OrderDto.CourierId)}
                    ,location_x     AS {nameof(OrderDto.LocationX)}
                    ,location_y     AS {nameof(OrderDto.LocationY)}
                    FROM public.orders
                    WHERE status != @status;";

            var command = new CommandDefinition(query, new { status = OrderStatus.Completed.Name }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<OrderDto>(command);
        }
    }
}
