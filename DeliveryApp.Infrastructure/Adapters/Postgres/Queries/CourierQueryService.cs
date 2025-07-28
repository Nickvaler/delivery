using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.Extensions.Options;
using Npgsql;
using static DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers.GetAllCouriersQueryResponse;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Queries
{
    public class CourierQueryService(IOptions<Settings> settings) : ICourierQueryService
    {
        public async Task<IEnumerable<CourierDto>> GetAllCouriersAsync(CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(settings.Value.ConnectionString);
            await connection.OpenAsync(cancellationToken);

            var query = @$"
                    SELECT 
                    id              AS {nameof(CourierDto.Id)}
                    ,name           AS {nameof(CourierDto.Name)}
                    ,location_x     AS {nameof(CourierDto.LocationX)}
                    ,location_y     AS {nameof(CourierDto.LocationY)}
                    FROM public.couriers
                    WHERE status != @status;";

            var command = new CommandDefinition(query, new { status = OrderStatus.Completed.Name }, cancellationToken: cancellationToken);
            return await connection.QueryAsync<CourierDto>(command);
        }
    }
}
