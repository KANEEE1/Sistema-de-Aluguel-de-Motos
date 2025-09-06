using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalSystem.Api.Dtos
{
    public class CreateMotorcycleDto
    {
        [Required]
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Ano must be between 1900 and 2100")]
        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Modelo must be between 2 and 50 characters")]
        [JsonPropertyName("modelo")]
        public string Model { get; set; } = string.Empty;

        [Required]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Placa must be exactly 7 characters")]
        [JsonPropertyName("placa")]
        public string Plate { get; set; } = string.Empty;
    }
}
