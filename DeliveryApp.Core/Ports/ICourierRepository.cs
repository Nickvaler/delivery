using DeliveryApp.Core.Domain.Models.CourierAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface ICourierRepository : IRepository<Courier>
    {
        Task AddAsync(Courier courier);
        void Update(Courier courier);
        Task<Courier> GetAsync(Guid id);
        Task<List<Courier>> GetAllAvailableAsync();
    }
}
