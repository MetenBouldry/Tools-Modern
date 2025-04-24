using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET Abstract Class representing a Patchouli Page
    /// </summary>
    public abstract class PatchouliPage
    {
        [JsonPropertyName("type")]
        public abstract string Type { get; }

        [JsonPropertyName("advancement")]
        public string? Advancement { get; set; }

        [JsonPropertyName("anchor")]
        public string? Anchor { get; set; }
    }
}
