using DeliveryApp.Core.Ports;
using MediatR;

namespace DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers
{
    public class GetAllCouriersQueryHandler(ICourierQueryService courierQueryService) : IRequestHandler<GetAllCouriersQuery, GetAllCouriersQueryResponse>
    {
        public async Task<GetAllCouriersQueryResponse> Handle(GetAllCouriersQuery request, CancellationToken cancellationToken)
        {
            var results = await courierQueryService.GetAllCouriersAsync(cancellationToken);
            return new GetAllCouriersQueryResponse(results);
        }
    }
}
