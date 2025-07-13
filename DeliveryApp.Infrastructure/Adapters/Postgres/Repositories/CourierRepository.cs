using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Ports;
using Microsoft.EntityFrameworkCore;

namespace DeliveryApp.Infrastructure.Adapters.Postgres.Repositories
{
    public class CourierRepository(ApplicationDbContext context) : ICourierRepository
    {
        public async Task AddAsync(Courier courier)
        {
            await context.AddAsync(courier);
        }

        public Task<Courier> GetAsync(Guid id)
        {
            return context.Couriers
                .Include(c => c.StoragePlaces)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public void Update(Courier courier)
        {
            context.Update(courier);
        }

        public Task<List<Courier>> GetAllAvailableAsync()
        {
            return context.Couriers
                .Include(c => c.StoragePlaces)
                .Where(o => o.StoragePlaces.TrueForAll(c => c.OrderId == null))
                .ToListAsync();
        }
    }
}
