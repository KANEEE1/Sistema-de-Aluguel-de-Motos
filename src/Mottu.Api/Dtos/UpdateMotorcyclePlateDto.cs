using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Mottu.Api.Dtos
{
    public class UpdateMotorcyclePlateDto
    {
        [Required]
        [StringLength(7, MinimumLength = 7, ErrorMessage = "Placa must be exactly 7 characters")]
        [JsonPropertyName("placa")]
        public string Plate { get; set; } = string.Empty;
    }
}
