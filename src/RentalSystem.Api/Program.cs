using Microsoft.EntityFrameworkCore;
using MassTransit;
using RentalSystem.Api.Data;
using RentalSystem.Api.Services;
using RentalSystem.Api.Consumers;
using RentalSystem.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RentalSystemDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<Motorcycle2024Consumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        cfg.ReceiveEndpoint("motorcycle-2024-queue", e =>
        {
            e.ConfigureConsumer<Motorcycle2024Consumer>(context);
        });
    });
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMotorcycleService, MotorcycleService>();
builder.Services.AddScoped<IDeliveryPersonService, DeliveryPersonService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<SimpleAuthMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();
