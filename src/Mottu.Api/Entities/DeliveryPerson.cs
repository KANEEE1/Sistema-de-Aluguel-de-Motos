using System;
using System.Text.Json.Serialization;
using Mottu.Api.Enums;

namespace Mottu.Api.Entities
{
    public class DeliveryPerson
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;

        [JsonPropertyName("data_nascimento")]
        public DateOnly BirthDate { get; set; }

        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;

        [JsonPropertyName("tipo_cnh")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public CnhType CnhType { get; set; }

        [JsonPropertyName("imagem_cnh")]
        public string? CnhImage { get; set; }

        public Guid Id { get; set; }
    }
}
