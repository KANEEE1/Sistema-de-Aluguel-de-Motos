using MassTransit;
using Microsoft.Extensions.Logging;
using RentalSystem.Api.Contracts;
using RentalSystem.Api.Data;
using RentalSystem.Api.Entities;

namespace RentalSystem.Api.Consumers
{
    public class Motorcycle2024Consumer : IConsumer<MotorcycleCreatedEvent>
    {
        private readonly RentalSystemDbContext _context;
        private readonly ILogger<Motorcycle2024Consumer> _logger;

        public Motorcycle2024Consumer(RentalSystemDbContext context, ILogger<Motorcycle2024Consumer> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MotorcycleCreatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Processing motorcycle event for ID {MotorcycleId}, Year {Year}", message.Id, message.Year);

            if (message.Year == 2024)
            {
                _logger.LogInformation("Motorcycle {MotorcycleId} is from 2024, creating notification", message.Id);

                var notification = new MotorcycleNotification
                {
                    Id = Guid.NewGuid(),
                    MotorcycleId = message.Id,
                    Year = message.Year,
                    Model = message.Model,
                    Plate = message.Plate,
                    CreatedAt = message.CreatedAt,
                    NotificationType = "MOTORCYCLE_CREATED_2024"
                };

                await _context.MotorcycleNotifications.AddAsync(notification);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Notification created for motorcycle {MotorcycleId} from 2024", message.Id);
            }
        }
    }
}
