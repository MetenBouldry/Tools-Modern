using System.Text.Json.Serialization;

namespace OresToFieldGuide
{
	public class MineralData
	{
		[JsonPropertyName("minerals")]
		public required Mineral[] Minerals { get; set; }
	}

	public class Mineral
	{
		[JsonPropertyName("id")]
		public required string ID { get; set; }

		[JsonPropertyName("name")]
		public string? Name { get; set; }

		[JsonPropertyName("use")]
		public required string Use { get; set; }

		[JsonPropertyName("for")]
		public required string[] For { get; set; }

		[JsonPropertyName("formula")]
		public string? Formula { get; set; }
	}
}
