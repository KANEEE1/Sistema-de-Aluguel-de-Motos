using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalSystem.Api.Dtos
{
    public class UpdateCnhImageDto
    {
        [Required]
        [JsonPropertyName("imagem_cnh")]
        public string CnhImage { get; set; } = string.Empty;
    }
}
