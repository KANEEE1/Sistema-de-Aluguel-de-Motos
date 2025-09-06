using System;
using System.Text.Json.Serialization;

namespace RentalSystem.Api.Entities
{
    public class Rental
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("valor_diaria")]
        public decimal DailyRate { get; set; }

        [JsonPropertyName("entregador_id")]
        public string DeliveryPersonIdentifier { get; set; } = string.Empty;

        [JsonPropertyName("identificador_moto")]
        public string MotorcycleIdentifier { get; set; } = string.Empty;

        [JsonPropertyName("data_inicio")]
        public DateOnly StartDate { get; set; }

        [JsonPropertyName("data_termino")]
        public DateOnly EndDate { get; set; }

        [JsonPropertyName("data_previsao_termino")]
        public DateOnly ExpectedEndDate { get; set; }

        [JsonPropertyName("data_devolucao")]
        public DateOnly? ReturnDate { get; set; }

        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        
        [JsonPropertyName("entregador_id_interno")]
        public Guid DeliveryPersonId { get; set; }
        
        [JsonPropertyName("moto_id_interno")]
        public Guid MotorcycleId { get; set; }
        
        [JsonPropertyName("valor_total")]
        public decimal TotalCost { get; set; }
    }
}
