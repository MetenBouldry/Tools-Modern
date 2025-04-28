using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class Ore : IDataJsonObject
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
        public required string DefaultIndicator { get; set; }

        /// <summary>
        /// Localized names and information for the ore.
        /// </summary>
        [JsonPropertyName("translations")]
        public required Translation[] RawTranslations { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> TranslatedNames { get; } = [];

        [JsonIgnore]
        public Dictionary<string, string?> TranslatedInfo { get; } = [];

		public Multiblock BuildMultiblockDisplay()
		{
			return new Multiblock()
			{
				Mapping = new Dictionary<string, string>
				{
					["0"] = $"#forge:ores/{ID}"
				},
				Pattern = [
					["0"],
					[" "]
				]
			};
		}
	}
}
