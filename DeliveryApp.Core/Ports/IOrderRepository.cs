using CSharpFunctionalExtensions;
using DeliveryApp.Core.Domain.Models.OrderAggregate;
using Primitives;

namespace DeliveryApp.Core.Ports
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task AddAsync(Order order);
        void Update(Order order);
        Task<Order> GetAsync(Guid id);
        Task<Order> GetRandomCreatedAsync();
        Task<List<Order>> GetAllAssignedAsync();
        Task<Maybe<Order>> GetFirstInCreatedStatusAsync();
    }
}
