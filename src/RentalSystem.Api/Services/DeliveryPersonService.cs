using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RentalSystem.Api.Data;
using RentalSystem.Api.Dtos;
using RentalSystem.Api.Entities;
using RentalSystem.Api.Enums;

namespace RentalSystem.Api.Services
{
    public class DeliveryPersonService : IDeliveryPersonService
    {
        private readonly RentalSystemDbContext _context;
        private readonly ILogger<DeliveryPersonService> _logger;

        public DeliveryPersonService(RentalSystemDbContext context, ILogger<DeliveryPersonService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<DeliveryPerson> CreateDeliveryPersonAsync(CreateDeliveryPersonDto createDeliveryPersonDto)
        {
            _logger.LogInformation("Criando entregador com CNPJ {Cnpj}", createDeliveryPersonDto.Cnpj);

            var today = DateOnly.FromDateTime(DateTime.Today);
            var age = today.Year - createDeliveryPersonDto.BirthDate.Year;
            if (createDeliveryPersonDto.BirthDate > today.AddYears(-age)) age--;
            if (age < 18)
                throw new InvalidOperationException("Entregador deve ter pelo menos 18 anos");

            if (!await IsCnpjUniqueAsync(createDeliveryPersonDto.Cnpj))
                throw new InvalidOperationException($"CNPJ {createDeliveryPersonDto.Cnpj} já existe");

            if (!await IsCnhNumberUniqueAsync(createDeliveryPersonDto.CnhNumber))
                throw new InvalidOperationException($"Número da CNH {createDeliveryPersonDto.CnhNumber} já existe");

            if (!await IsIdentifierUniqueAsync(createDeliveryPersonDto.Identifier))
                throw new InvalidOperationException($"Identificador {createDeliveryPersonDto.Identifier} já existe");

            if (!Enum.IsDefined(typeof(CnhType), createDeliveryPersonDto.CnhType))
                throw new InvalidOperationException("Tipo de CNH inválido");

            var deliveryPerson = new DeliveryPerson
            {
                Id = Guid.NewGuid(),
                Identifier = createDeliveryPersonDto.Identifier,
                Name = createDeliveryPersonDto.Name,
                Cnpj = createDeliveryPersonDto.Cnpj,
                BirthDate = createDeliveryPersonDto.BirthDate,
                CnhNumber = createDeliveryPersonDto.CnhNumber,
                CnhType = createDeliveryPersonDto.CnhType,
                CnhImage = createDeliveryPersonDto.CnhImage
            };

            await _context.DeliveryPeople.AddAsync(deliveryPerson);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Entregador criado com sucesso com ID {EntregadorId}", deliveryPerson.Id);

            return deliveryPerson;
        }

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id)
        {
            return await _context.DeliveryPeople.FindAsync(id);
        }

        public async Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string identifier)
        {
            return await _context.DeliveryPeople
                .FirstOrDefaultAsync(dp => dp.Identifier == identifier);
        }

        public async Task<IEnumerable<DeliveryPerson>> ListDeliveryPeopleAsync()
        {
            return await _context.DeliveryPeople.ToListAsync();
        }

        public async Task<bool> UpdateCnhImageAsync(Guid id, string cnhImage)
        {
            var deliveryPerson = await _context.DeliveryPeople.FindAsync(id);
            if (deliveryPerson == null)
                return false;

            deliveryPerson.CnhImage = cnhImage;
            _context.DeliveryPeople.Update(deliveryPerson);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> IsCnpjUniqueAsync(string cnpj, Guid? excludeId = null)
        {
            var query = _context.DeliveryPeople.Where(dp => dp.Cnpj == cnpj);

            if (excludeId.HasValue)
                query = query.Where(dp => dp.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsCnhNumberUniqueAsync(string cnhNumber, Guid? excludeId = null)
        {
            var query = _context.DeliveryPeople.Where(dp => dp.CnhNumber == cnhNumber);

            if (excludeId.HasValue)
                query = query.Where(dp => dp.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsIdentifierUniqueAsync(string identifier, Guid? excludeId = null)
        {
            var query = _context.DeliveryPeople.Where(dp => dp.Identifier == identifier);

            if (excludeId.HasValue)
                query = query.Where(dp => dp.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsQualifiedForRentalAsync(Guid deliveryPersonId)
        {
            var deliveryPerson = await _context.DeliveryPeople.FindAsync(deliveryPersonId);
            if (deliveryPerson == null)
                return false;

            return deliveryPerson.CnhType == CnhType.A || deliveryPerson.CnhType == CnhType.AB;
        }
    }
}