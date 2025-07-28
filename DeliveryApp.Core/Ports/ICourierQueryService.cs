using static DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers.GetAllCouriersQueryResponse;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierQueryService
    {
        Task<IEnumerable<CourierDto>> GetAllCouriersAsync(CancellationToken cancellationToken);
    }
}
