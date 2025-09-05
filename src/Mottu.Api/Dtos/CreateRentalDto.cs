using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mottu.Api.Enums;

namespace Mottu.Api.Dtos
{
    public class CreateRentalDto
    {
        [Required]
        [JsonPropertyName("idetificador_entregador")]
        public string DeliveryPersonIdentifier { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("identificador_moto")]
        public string MotorcycleIdentifier { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("data_inicio")]
        public DateOnly StartDate { get; set; }

        [Required]
        [JsonPropertyName("data_termino")]
        public DateOnly EndDate { get; set; }

        [Required]
        [JsonPropertyName("data_previsao_termino")]
        public DateOnly ExpectedEndDate { get; set; }

        [Required]
        [JsonPropertyName("plano")]
        public RentalPlan Plan { get; set; }
    }
}