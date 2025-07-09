using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class OrderRepository(ApplicationDbContext context) : IOrderRepository
    {
        public async Task AddAsync(Order order)
        {
            await context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            context.Orders.Update(order);
        }

        public async Task<Order> GetAsync(Guid id)
        {
            return await context.Orders
                .FirstOrDefaultAsync(o => o.Id == id);
        }
        public async Task<List<Order>> GetAllAssignedAsync()
        {
            return await context.Orders
                .Where(o => o.Status == OrderStatus.Assigned)
                .ToListAsync();
        }

        public async Task<Order> GetRandomCreatedAsync()
        {
            return await context.Orders
                .OrderBy(o => Random.Shared.Next())
                .FirstOrDefaultAsync();
        }
    }
}
