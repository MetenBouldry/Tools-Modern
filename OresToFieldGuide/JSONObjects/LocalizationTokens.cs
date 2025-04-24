using System.Text.Json;
using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
    public class LocalizationTokens
    {
        public static async Task<LocalizationTokens> FromJSON(string jsonFilePath)
        {
            string json = await File.ReadAllTextAsync(jsonFilePath);

            var localizationTokens = JsonSerializer.Deserialize<LocalizationTokens>(json);
            localizationTokens!.LocaleName = Path.GetFileNameWithoutExtension(jsonFilePath);
            return localizationTokens;
        }

        public required string LocaleName { get; set; }

        [JsonPropertyName("ore_index_format")]
        public required string OreIndex { get; set; }

        [JsonPropertyName("vein_index_format")]
        public required string VeinIndex { get; set; }

        [JsonPropertyName("planet_names")]
        public required Dictionary<string, string> PlanetDictionary { get; set; }

        [JsonPropertyName("keywords")]
        public required Dictionary<string, string> KeywordDictionary { get; set; }

        [JsonPropertyName("rocks")]
        public required Dictionary<string, string> RockDictionary { get; set; }

        [JsonPropertyName("veins")]
        public required Dictionary<string, string> VeinTypeDictionary { get; set; }
    }
}
