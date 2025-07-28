using CSharpFunctionalExtensions;
using DeliveryApp.Api;
using DeliveryApp.Core.Application.UseCases.Commands.AssignToCourier;
using DeliveryApp.Core.Application.UseCases.Commands.CreateOrder;
using DeliveryApp.Core.Application.UseCases.Commands.MoveCourier;
using DeliveryApp.Core.Application.UseCases.Queries.GetAllCouriers;
using DeliveryApp.Core.Application.UseCases.Queries.GetCreatedAndAssignedOrders;
using DeliveryApp.Core.Domain.Services;
using DeliveryApp.Core.Ports;
using DeliveryApp.Infrastructure.Adapters.Postgres;
using DeliveryApp.Infrastructure.Adapters.Postgres.Queries;
using DeliveryApp.Infrastructure.Adapters.Postgres.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Primitives;

var builder = WebApplication.CreateBuilder(args);

// Health Checks
builder.Services.AddHealthChecks();

// Cors
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin(); // Не делайте так в проде!
        });
});

builder.Services.AddTransient<IDispatchService, DispatchService>();

// Configuration
builder.Services.ConfigureOptions<SettingsSetup>();

var connectionString = builder.Configuration["CONNECTION_STRING"];

// БД, ORM 
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString,
        sqlOptions => { sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"); });
    options.EnableSensitiveDataLogging();
}
);

// UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Repositories
builder.Services.AddScoped<ICourierRepository, CourierRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Commands 
builder.Services.AddScoped<IRequestHandler<AssignToCourierCommand, UnitResult<Error>>, AssignToCourierCommandHandler>();
builder.Services.AddScoped<IRequestHandler<CreateOrderCommand, UnitResult<Error>>, CreateOrderCommandHandler>();
builder.Services.AddScoped<IRequestHandler<MoveCourierCommand, UnitResult<Error>>, MoveCourierCommandHandler>();

// Query Services

builder.Services.AddScoped<ICourierQueryService, CourierQueryService>();
builder.Services.AddScoped<IOrderQueryService, OrderQueryService>();

// Queries
builder.Services.AddScoped<IRequestHandler<GetAllCouriersQuery, GetAllCouriersQueryResponse>, GetAllCouriersQueryHandler>();
builder.Services.AddScoped<IRequestHandler<GetCreatedAndAssignedOrdersQuery, GetCreatedAndAssignedOrdersQueryResponse>, GetCreatedAndAssignedOrdersQueryHandler>();

var app = builder.Build();

// -----------------------------------
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseHsts();

app.UseHealthChecks("/health");
app.UseRouting();

// Apply Migrations
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();