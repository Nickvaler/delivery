using Dapper;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using MediatR;
using Npgsql;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAssignedCouriers
{
    public class GetAssignedCouriersQueryHandler : IRequestHandler<GetAssignedCouriersQuery, GetAssignedCouriersQueryResponse>
    {
        private readonly string _connectionString;

        public GetAssignedCouriersQueryHandler(string connectionString)
        {
            _connectionString = !string.IsNullOrWhiteSpace(connectionString)
                ? connectionString
                : throw new ArgumentNullException(nameof(connectionString));
        }


        public async Task<GetAssignedCouriersQueryResponse> Handle(GetAssignedCouriersQuery request, CancellationToken cancellationToken)
        {
            await using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            var results = await connection.QueryAsync<GetAssignedCouriersQueryResponse.Courier>(
                    @$"
                    SELECT 
                    id              AS {nameof(GetAssignedCouriersQueryResponse.Courier.Id)}
                    ,name           AS {nameof(GetAssignedCouriersQueryResponse.Courier.Name)}
                    ,location_x     AS {nameof(GetAssignedCouriersQueryResponse.Courier.LocationX)}
                    ,location_y     AS {nameof(GetAssignedCouriersQueryResponse.Courier.LocationY)}
                    FROM public.couriers
                    WHERE status != @status;"
                , new { status = OrderStatus.Completed.Name });

            return new GetAssignedCouriersQueryResponse(results.ToList());
        }
    }
}
