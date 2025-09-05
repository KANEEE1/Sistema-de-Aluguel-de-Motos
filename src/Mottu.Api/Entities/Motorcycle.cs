using System;
using System.Text.Json.Serialization;

namespace Mottu.Api.Entities
{
    public class Motorcycle
    {
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [JsonPropertyName("modelo")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string Plate { get; set; } = string.Empty;

        public Guid Id { get; set; }
    }
}
