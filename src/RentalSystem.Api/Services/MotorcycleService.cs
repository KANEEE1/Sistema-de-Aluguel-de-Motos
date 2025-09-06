using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalSystem.Api.Contracts;
using RentalSystem.Api.Data;
using RentalSystem.Api.Dtos;
using RentalSystem.Api.Entities;

namespace RentalSystem.Api.Services
{
    public class MotorcycleService : IMotorcycleService
    {
        private readonly RentalSystemDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<MotorcycleService> _logger;

        public MotorcycleService(RentalSystemDbContext context, IPublishEndpoint publishEndpoint, ILogger<MotorcycleService> logger)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task<Motorcycle> CreateMotorcycleAsync(CreateMotorcycleDto createMotorcycleDto)
        {
            _logger.LogInformation("Criando moto com placa {Placa}", createMotorcycleDto.Plate);

            if (!await IsIdentifierUniqueAsync(createMotorcycleDto.Identifier))
                throw new InvalidOperationException($"Identificador {createMotorcycleDto.Identifier} já existe");

            if (!await IsPlateUniqueAsync(createMotorcycleDto.Plate))
                throw new InvalidOperationException($"Placa {createMotorcycleDto.Plate} já existe");

            var motorcycle = new Motorcycle
            {
                Id = Guid.NewGuid(),
                Identifier = createMotorcycleDto.Identifier,
                Year = createMotorcycleDto.Year,
                Model = createMotorcycleDto.Model,
                Plate = createMotorcycleDto.Plate.ToUpper()
            };

            await _context.Motorcycles.AddAsync(motorcycle);
            await _context.SaveChangesAsync();

            var motorcycleCreatedEvent = new MotorcycleCreatedEvent
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year,
                Model = motorcycle.Model,
                Plate = motorcycle.Plate,
                CreatedAt = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(motorcycleCreatedEvent);
            _logger.LogInformation("Moto criada e evento publicado para ID {MotoId}", motorcycle.Id);

            return motorcycle;
        }

        public async Task<Motorcycle?> GetMotorcycleByIdAsync(Guid id)
        {
            return await _context.Motorcycles.FindAsync(id);
        }

        public async Task<IEnumerable<Motorcycle>> ListMotorcyclesAsync(string? plate = null)
        {
            var query = _context.Motorcycles.AsQueryable();

            if (!string.IsNullOrEmpty(plate))
                query = query.Where(m => m.Plate.Contains(plate.ToUpper()));

            return await query.ToListAsync();
        }

        public async Task<bool> UpdateMotorcyclePlateByIdAsync(Guid id, UpdateMotorcyclePlateDto updateDto)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null)
                return false;

            if (!await IsPlateUniqueAsync(updateDto.Plate, id))
                throw new InvalidOperationException($"Placa {updateDto.Plate} já existe");

            motorcycle.Plate = updateDto.Plate.ToUpper();
            _context.Motorcycles.Update(motorcycle);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Placa da moto {MotoId} alterada para {PlacaNova}", id, updateDto.Plate);

            return true;
        }

        public async Task<bool> DeleteMotorcycleAsync(Guid id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle == null)
                return false;

            var hasActiveRentals = await _context.Rentals.AnyAsync(r => r.MotorcycleId == id && r.ReturnDate == null);
            if (hasActiveRentals)
                throw new InvalidOperationException("Não é possível deletar moto com locações ativas");

            _context.Motorcycles.Remove(motorcycle);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Moto {MotoId} removida com sucesso", id);

            return true;
        }

        public async Task<bool> IsPlateUniqueAsync(string plate, Guid? excludeId = null)
        {
            var query = _context.Motorcycles.Where(m => m.Plate == plate.ToUpper());

            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsIdentifierUniqueAsync(string identifier, Guid? excludeId = null)
        {
            var query = _context.Motorcycles.Where(m => m.Identifier == identifier);

            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);

            return !await query.AnyAsync();
        }
    }
}