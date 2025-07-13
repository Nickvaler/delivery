using DeliveryApp.Core.Domain.Models.CourierAggregate;
using DeliveryApp.Core.Domain.Models.SharedKernel;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Repositories
{
    public class CourierRepositoryShould : IAsyncLifetime
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
        public async Task CanAddCourier()
        {
            //  Arrange
            var location = Location.CreateRandom();
            var speed = 5;
            var courierCreateResult = Courier.Create("Test", speed, location.Value);

            //  Act
            var courierRepository = new CourierRepository(_context);
            await courierRepository.AddAsync(courierCreateResult.Value);
            var unitOfWork = new UnitOfWork(_context);
            await unitOfWork.SaveChangesAsync();

            //  Assert
            var courierFromDb = await courierRepository.GetAsync(courierCreateResult.Value.Id);
            courierCreateResult.Value.Should().BeEquivalentTo(courierFromDb);
        }
    }
}
