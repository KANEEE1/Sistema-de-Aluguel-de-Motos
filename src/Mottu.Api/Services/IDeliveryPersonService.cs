using Mottu.Api.Dtos;
using Mottu.Api.Entities;

namespace Mottu.Api.Services
{
    public interface IDeliveryPersonService
    {
        Task<DeliveryPerson> CreateDeliveryPersonAsync(CreateDeliveryPersonDto createDeliveryPersonDto);
        Task<DeliveryPerson?> GetDeliveryPersonByIdAsync(Guid id);
        Task<DeliveryPerson?> GetDeliveryPersonByIdentifierAsync(string identifier);
        Task<IEnumerable<DeliveryPerson>> ListDeliveryPeopleAsync();
        Task<bool> UpdateCnhImageAsync(Guid id, string cnhImage);
        Task<bool> IsCnpjUniqueAsync(string cnpj, Guid? excludeId = null);
        Task<bool> IsCnhNumberUniqueAsync(string cnhNumber, Guid? excludeId = null);
        Task<bool> IsIdentifierUniqueAsync(string identifier, Guid? excludeId = null);
        Task<bool> IsQualifiedForRentalAsync(Guid deliveryPersonId);
    }
}
