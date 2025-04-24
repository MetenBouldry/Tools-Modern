using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class Vein
    {
        /// <summary>
        /// A unique identifier for this vein. Also used for the "random_name" property as a seed.
        /// </summary>
        [JsonPropertyName("id")]
        public required string ID { get; set; }

        /// <summary>
        /// What type of Vein this is, IE: tfc:disc_vein
        /// </summary>
        [JsonPropertyName("type")]
        public required string Type { get; set; }

        /// <summary>
        /// The Configuration for the Vein
        /// </summary>
        [JsonPropertyName("config")]
        public required VeinConfig Config { get; set; }

        /// <summary>
        /// The ores and their weights
        /// </summary>
        [JsonPropertyName("ores")]
        public required WeightedBlock[] Ores { get; set; }

        /// <summary>
        /// An array of <see cref="Rock.ID"/>s that this vein can appear in
        /// </summary>
        [JsonPropertyName("rocks")]
        public required string[] Rocks { get; set; }
    }

    public class VeinConfig
    {
        /// <summary>
        /// The Rarity of the vein
        /// </summary>
        [JsonPropertyName("rarity")]
        public required float Rarity { get; set; }

        /// <summary>
        /// The Density of the vein
        /// </summary>
        [JsonPropertyName("Density")]
        public required float Density { get; set; }

        /// <summary>
        /// The min Y value at which this vein can spawn
        /// </summary>
        [JsonPropertyName("min_y")]
        public required float MinY { get; set; }

        /// <summary>
        /// The max Y value at which this vein can spawn
        /// </summary>
        [JsonPropertyName("max_y")]
        public required float MaxY { get; set; }

        /// <summary>
        /// The Size of the Vein
        /// </summary>
        [JsonPropertyName("size")]
        public float? Size { get; set; }

        /// <summary>
        /// The Height of the Vein
        /// </summary>
        [JsonPropertyName("height")]
        public float? Height { get; set; }

        /// <summary>
        /// The Radius of the Vein
        /// </summary>
        [JsonPropertyName("radius")]
        public float? Radius { get; set; }
    }

    public class WeightedBlock
    {
        /// <summary>
        /// The <see cref="Ore.ID"/> of the ore to use
        /// </summary>
        [JsonPropertyName("ore")]
        public required string Block { get; set; }

        /// <summary>
        /// The weight for this block, ranges between 0 and 100
        /// </summary>
        [JsonPropertyName("weight")]
        public required float Weight { get; set; }

        /// <summary>
        /// If this is non-null, add another entry to the vein with this weight and this ore's <see cref="Ore.OreBlock"/>
        /// </summary>
        [JsonPropertyName("block_weight")]
        public float? BlockWeight { get; set; }
    }

    public class IndicatorConfig
    {
        /// <summary>
        /// The rarity of how often this indicator appears on the surface, as 1 in N blocks
        /// </summary>
        [JsonPropertyName("rarity")]
        public required int Rarity { get; set; }

        /// <summary>
        /// How close the vein has to be to the surface for it to get surface indicators
        /// </summary>
        [JsonPropertyName("depth")]
        public required int Depth { get; set; }

        /// <summary>
        /// The rarity of how often this indicator appears within caves, as 1 in N blocks
        /// (This includes air blocks! So this number should always be significantly higher than <see cref="Rarity"/>)
        /// </summary>
        [JsonPropertyName("underground_rarity")]
        public required int UndergroundRarity { get; set; }

        /// <summary>
        /// How many times placing an underground indicator should be attempted
        /// </summary>
        [JsonPropertyName("underground_count")]
        public required int UndergroundCount { get; set; }

        /// <summary>
        /// The indicator blocks and their weights to use for this vein
        /// </summary>
        [JsonPropertyName("blocks")]
        public required WeightedIndicator[] Blocks { get; set; }
    }

    public class WeightedIndicator
    {
        /// <summary>
        /// A block ID for what to use as an indicator, like "gtceu:coal_indicator" or "gtceu:ruby_bud_indicator" or "tfc:ore/small_tetrahedrite"
        /// </summary>
        [JsonPropertyName("block")]
        public required string Block { get; set; }

        /// <summary>
        /// The weight for this block, ranges between 0 and 100
        /// </summary>
        [JsonPropertyName("weight")]
        public required float Weight { get; set; }
    }
}