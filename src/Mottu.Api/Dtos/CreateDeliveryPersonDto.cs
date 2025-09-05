using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Mottu.Api.Enums;

namespace Mottu.Api.Dtos
{
    public class CreateDeliveryPersonDto
    {
        [Required]
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 100 caracteres")]
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "CNPJ deve ter exatamente 14 caracteres")]
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("data_nascimento")]
        public DateOnly BirthDate { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Número da CNH deve ter exatamente 11 caracteres")]
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("tipo_cnh")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CnhType CnhType { get; set; }

        [JsonPropertyName("imagem_cnh")]
        public string? CnhImage { get; set; } = null;
    }
}
