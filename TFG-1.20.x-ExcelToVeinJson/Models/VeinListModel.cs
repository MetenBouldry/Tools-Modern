using Newtonsoft.Json;

namespace TFG_1._20.x_ExcelToVeinJson.Models;

[JsonObject(ItemNullValueHandling = NullValueHandling.Ignore)]
public class VeinListModel
{
    [JsonProperty("replace")] 
    public bool Replace { get; set; } = true;
    
    [JsonProperty("values")] 
    public string[] Values { get; set; }
}