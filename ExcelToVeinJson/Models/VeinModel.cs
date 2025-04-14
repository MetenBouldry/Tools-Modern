using Newtonsoft.Json;

namespace TFG_1._20.x_ExcelToVeinJson.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class VeinModel
{
    [JsonProperty("type")]
    public string Type { get; set; }
    [JsonProperty("config")]
    public Config Config { get; set; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed class Config
{
    [JsonProperty("rarity")]
    public int Rarity { get; set; }
    
    [JsonProperty("density")]
    public float Density { get; set; }
    
    [JsonProperty("min_y")]
    public int MinY { get; set; }
    
    [JsonProperty("max_y")]
    public int MaxY { get; set; }
    
    [JsonProperty("size")]
    public int? Size { get; set; }
    
    [JsonProperty("random_name")]
    public string RandomName { get; set; }
    
    [JsonProperty("min_skew")]
    public int? MinSkew { get; set; }
    
    [JsonProperty("max_skew")]
    public int? MaxSkew { get; set; }
    
    [JsonProperty("min_slant")]
    public int? MinSlant { get; set; }
    
    [JsonProperty("max_slant")]
    public int? MaxSlant { get; set; }
    
    [JsonProperty("sign")]
    public int? Sign { get; set; }
    
    [JsonProperty("height")]
    public int? Height { get; set; }
    
    [JsonProperty("radius")]
    public int? Radius { get; set; }
    
    [JsonProperty("project")]
    public bool? Project { get; set; } = true;
    
    [JsonProperty("project_offset")]
    public bool? ProjectOffset { get; set; } = true;
    
    [JsonProperty("biomes")]
    public string? Biomes { get; set; }
    
    [JsonProperty("blocks")]
    public OreBlocks[] Blocks { get; set; }
    
    [JsonProperty("indicator")]
    public Indicator? Indicator { get; set; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed class OreBlocks
{
    [JsonProperty("replace")]
    public string[] Replace { get; set; }
    [JsonProperty("with")]
    public BlockEntry[] With { get; set; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed class BlockEntry
{
    [JsonProperty("block")]
    public string Block { get; set; }
    [JsonProperty("weight")]
    public int? Weight { get; set; }
}

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public sealed class Indicator
{
    [JsonProperty("rarity")]
    public int? Rarity { get; set; }
    [JsonProperty("depth")]
    public int? Depth { get; set; }
    [JsonProperty("underground_rarity")]
    public int? UndergroundRarity { get; set; }
    [JsonProperty("underground_count")]
    public int? UndergroundCount { get; set; }
    [JsonProperty("blocks")]
    public BlockEntry[] Blocks { get; set; }
}