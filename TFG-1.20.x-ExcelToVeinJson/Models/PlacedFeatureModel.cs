using Newtonsoft.Json;

namespace TFG_1._20.x_ExcelToVeinJson.Models;

public class PlacedFeatureModel
{
    [JsonProperty("feature")]
    public string Feature { get; set; }

    [JsonProperty("placement")] 
    public string[] Placement { get; set; } = Array.Empty<string>();
}