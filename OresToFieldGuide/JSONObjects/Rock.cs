using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class Rock
    {
        [JsonPropertyName("id")]
        public required string ID { get; set; }

        [JsonPropertyName("pattern")]
        public required string Pattern { get; set; }

        [JsonPropertyName("replaceable_blocks")]
        public required string[] ReplaceableBlocks { get; set; }

        [JsonPropertyName("translations")]
        public required RockTranslation[] Translations { get; set; }
    }

    public class RockTranslation
    {
        [JsonPropertyName("lang")]
        public required string Lang { get; set; }

        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }
}
