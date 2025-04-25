﻿using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class Ore
    {
        [JsonPropertyName("id")]
        public required string ID { get; set; }

        /// <summary>
        /// Optional chemical formula of this ore.
        /// </summary>
        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        /// <summary>
        /// The name of the hazard associated with this ore, if any.
        /// </summary>
        [JsonPropertyName("hazard")]
        public string? Hazard { get; set; }

        /// <summary>
        /// Optional full block of ore to use in rich veins.
        /// </summary>
        [JsonPropertyName("ore_block")]
        public string? FullOreBlock { get; set; }

        /// <summary>
        /// The default indicator to use for this ore.
        /// </summary>
        [JsonPropertyName("indicator")]
        public string? DefaultIndicator { get; set; }

        /// <summary>
        /// Localized names and information for the ore.
        /// </summary>
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
