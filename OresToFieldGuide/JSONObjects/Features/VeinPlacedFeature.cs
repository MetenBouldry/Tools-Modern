using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class VeinPlacedFeature
	{
		[JsonPropertyName("feature")]
		public required string Feature { get; set; }

		/// <summary>
		/// Always empty unless we need to make a climate-sensitive vein or something
		/// </summary>
		[JsonPropertyName("placement")]
		public object[] Placement { get; } = [];
	}
}
