using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class Rock : IDataJsonObject
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("pattern")]
		public required string Pattern { get; set; }

		[JsonPropertyName("replaceable_blocks")]
		public required string[] ReplaceableBlocks { get; set; }

		[JsonPropertyName("translations")]
		public required Dictionary<string, string> Translations { get; set; }
	}
}
