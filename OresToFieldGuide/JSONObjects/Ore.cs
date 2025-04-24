using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class Ore
    {
        [JsonPropertyName("id")]
        public required string ID { get; set; }

        [JsonPropertyName("formula")]
        public required string Formula { get; set; }

        [JsonPropertyName("hazardous")]
        public bool Hazardous { get; set; } = false;

        [JsonPropertyName("ore_block")]
        public string? OreBlock { get; set; }

        [JsonPropertyName("indicator")]
        public string? Indicator { get; set; }

        [JsonPropertyName("translations")]
        public required OreTranslation[] Translations { get; set; }
    }

    public class OreTranslation
    {
        [JsonPropertyName("lang")]
        public required string Language { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("info")]
        public required string Info { get; set; }
    }
}
