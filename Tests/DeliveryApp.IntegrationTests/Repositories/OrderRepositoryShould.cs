using DeliveryApp.Core.Domain.Models.OrderAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class OrderRepositoryShould : IAsyncLifetime
    {
        private ApplicationDbContext _context;
        private readonly PostgreSqlContainer _container = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("delivery")
            .WithUsername("username")
            .WithPassword("password")
            .WithCleanUp(true)
            .Build();

        public async Task InitializeAsync()
        {
            await _container.StartAsync();
            var contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseNpgsql(_container.GetConnectionString(),
                options =>
                {
                    options.MigrationsAssembly("DeliveryApp.Infrastructure");
                }).Options;

            _context = new ApplicationDbContext(contextOptions);
            await _context.Database.MigrateAsync();
        }

        public async Task DisposeAsync()
        {
            await _container.DisposeAsync().AsTask();
        }

        [Fact]
        public async Task CanAddOrder()
        {
            //  Arrange
            var orderId = Guid.NewGuid();
            var location = Location.CreateRandom();
            var volume = 5;
            var orderCreateResult = Order.Create(orderId, location.Value, volume);

            //  Act
            var orderRepository = new OrderRepository(_context);
            await orderRepository.AddAsync(orderCreateResult.Value);
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //  Assert
            var orderFromDb = await orderRepository.GetAsync(orderId);
            orderCreateResult.Value.Should().BeEquivalentTo(orderFromDb);
        }
    }
}
