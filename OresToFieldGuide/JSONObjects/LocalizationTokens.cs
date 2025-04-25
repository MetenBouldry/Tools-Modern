using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class LocalizationTokens
    {
        [JsonIgnore]
        public string? LocaleName { get; set; }

        [JsonPropertyName("ore_index_format")]
        public required string OreIndex { get; set; }

        [JsonPropertyName("vein_index_format")]
        public required string VeinIndex { get; set; }

        [JsonPropertyName("dimensions")]
        public required Dictionary<string, string> Dimensions { get; set; }

        [JsonPropertyName("keywords")]
        public required Dictionary<string, string> Keywords { get; set; }
    }
}
