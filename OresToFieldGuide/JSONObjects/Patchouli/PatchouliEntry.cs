using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    /// <summary>
    /// A .NET represnetation of a Patchouli Entry
    /// <para></para>
    /// <a href="https://vazkiimods.github.io/Patchouli/docs/reference/entry-json">Patchouli Docs</a>
    /// </summary>
    [Serializable]
    public class PatchouliEntry
    {
        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonIgnore]
        public required string FileNameWithoutExtension { get; set; }

        [JsonPropertyName("icon")]
        public required string Icon { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; } = "tfc:tfg_ores";

        [JsonPropertyName("read_by_default")]
        public bool ReadByDefault { get; set; }

        [JsonPropertyName("priority")]
        public bool Priority { get; set; } = false;

        [JsonPropertyName("advancement")]
        public string? RequiredAdvancement { get; set; }

        [JsonPropertyName("secret")]
        public bool IsSecret { get; set; } = false;

        [JsonPropertyName("pages")]
        public required PatchouliPage[] Pages { get; set; }
    }
}