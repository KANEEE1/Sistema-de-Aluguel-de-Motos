using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace RentalSystem.Api.Dtos
{
    public class ReturnRentalDto
    {
        [Required]
        [JsonPropertyName("data_devolucao")]
        public DateOnly ReturnDate { get; set; }
    }
}
