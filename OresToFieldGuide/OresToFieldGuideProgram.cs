using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common;

namespace OresToFieldGuide
{
	internal class OresToFieldGuideProgram(ProgramArguments arguments)
	{
		public static string s_fallbackLocale => s_locales[0];
		public static readonly string[] s_locales =
		[
			"en_us", // US English
            "ru_ru", // Russian
            "uk_ua", // Ukranian
            //"it_it", // Italian
        ];

		private readonly JsonSerializerOptions m_jsonOptions = new()
		{
			WriteIndented = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
		};

		private readonly ProgramArguments m_arguments = arguments;

		private readonly Dictionary<string, Dimension> m_dimensionDict = [];
		private readonly Dictionary<string, Ore> m_oreDict = [];
		private readonly Dictionary<string, Rock> m_rockDict = [];
		private readonly Dictionary<Dimension, List<Vein>> m_veinDict = [];

		private readonly Dictionary<string, LocalizationTokens> m_localeToTokens = [];

		public void Run()
		{
			// 1) Load json
			DeserializeData("dimensions", m_dimensionDict);
			DeserializeData("ores", m_oreDict);
			DeserializeData("rock", m_rockDict);
			DeserializeVeins();
			DeserializeLanguageTokens();

			// 2) Reorganize data

			ExtractTranslations();
			WeightsIntoPercents();

			// 3) Write out veins

			ExportConfiguredVeins();
			ExportPlacedVeins();

			// 4) Write out patchouli

			ExportPatchouliEntries();
		}

		private void DeserializeData<T>(string subDir, Dictionary<string, T> dict) where T : IDataJsonObject
		{
			var paths = Path.Combine(m_arguments.DataFolder, subDir);
			foreach (var path in Directory.EnumerateFiles(paths))
			{
				var thing = JsonSerializer.Deserialize<T>(File.ReadAllText(path));
				if (thing == null)
				{
					ConsoleLogHelper.WriteLine($"Failed to parse ${path}, skipping", LogLevel.Error);
					continue;
				}

				dict[thing.ID] = thing;
			}
		}

		/// <summary>
		/// Deserializes all the veins and stores them in a dictionary of dims to veins
		/// </summary>
		private void DeserializeVeins()
		{
			var veinsPath = Path.Combine(m_arguments.DataFolder, "veins");
			foreach (var dimension in m_dimensionDict.Values)
			{
				List<Vein> deserializedVeins = [];

				foreach (string veinPath in Directory.EnumerateFiles(Path.Combine(veinsPath, dimension.ID)))
				{
					Vein? vein = JsonSerializer.Deserialize<Vein>(File.ReadAllText(veinPath));
					if (vein == null)
					{
						ConsoleLogHelper.WriteLine($"Failed to parse ${veinPath}, skipping", LogLevel.Error);
						continue;
					}

					deserializedVeins.Add(vein);
				}

				m_veinDict[dimension] = deserializedVeins;
			}
		}

		private void DeserializeLanguageTokens()
		{
			var en_usTokenPath = Path.Combine(m_arguments.DataFolder, "language_tokens", $"{s_fallbackLocale}.json");
			LocalizationTokens en_usTokens = JsonSerializer.Deserialize<LocalizationTokens>(File.ReadAllText(en_usTokenPath))
				?? throw new Exception($"Failed to parse {en_usTokenPath}");

			m_localeToTokens.Add(s_fallbackLocale, en_usTokens);

			foreach (var locale in s_locales)
			{
				if (m_localeToTokens.ContainsKey(locale))
					continue;

				var localeTokenPath = Path.Combine(m_arguments.DataFolder, "language_tokens", $"{locale}.json");
				if (!File.Exists(localeTokenPath))
				{
					ConsoleLogHelper.WriteLine($"Could not file localization tokens for locale {locale}! Assigning {s_fallbackLocale}'s Localization Tokens.", LogLevel.Warning);
					m_localeToTokens.Add(locale, en_usTokens);
					continue;
				}

				LocalizationTokens? localeTokens = JsonSerializer.Deserialize<LocalizationTokens>(File.ReadAllText(localeTokenPath));
				if (localeTokens == null)
				{
					ConsoleLogHelper.WriteLine($"Failed to parse ${localeTokenPath}! Assigning {s_fallbackLocale}'s tokens instead", LogLevel.Warning);
					m_localeToTokens.Add(locale, en_usTokens);
					continue;
				}

				m_localeToTokens.Add(locale, localeTokens);
			}
		}

		private void ExtractTranslations()
		{
			foreach (var dimension in m_dimensionDict.Values)
			{
				foreach (var locale in s_locales)
				{
					if (!dimension.Translations.ContainsKey(locale))
					{
						dimension.Translations[locale] = dimension.Translations[s_fallbackLocale];
					}
				}
			}

			foreach (var ore in m_oreDict.Values)
			{
				var dict = ore.RawTranslations.ToDictionary(t => t.Language);

				foreach (var locale in s_locales)
				{
					if (dict.TryGetValue(locale, out var translation))
					{
						ore.TranslatedNames[locale] = translation.Text;
						ore.TranslatedInfo[locale] = translation.Info;
					}
					else
					{
						ore.TranslatedNames[locale] = ore.TranslatedNames[s_fallbackLocale];
						ore.TranslatedInfo[locale] = ore.TranslatedInfo[s_fallbackLocale];
					}
				}
			}

			foreach (var rock in m_rockDict.Values)
			{
				foreach (var locale in s_locales)
				{
					if (!rock.Translations.ContainsKey(locale))
					{
						rock.Translations[locale] = rock.Translations[s_fallbackLocale];
					}
				}
			}

			foreach (var vein in m_veinDict.Values.SelectMany(v => v))
			{
				var dict = vein.RawTranslations.ToDictionary(t => t.Language);

				foreach (var locale in s_locales)
				{
					if (dict.TryGetValue(locale, out var translation))
					{
						vein.TranslatedNames[locale] = translation.Text;
						vein.TranslatedInfo[locale] = translation.Info;
					}
					else
					{
						vein.TranslatedNames[locale] = vein.TranslatedNames[s_fallbackLocale];
						vein.TranslatedInfo[locale] = vein.TranslatedInfo[s_fallbackLocale];
					}
				}
			}
		}

		private void WeightsIntoPercents()
		{
			foreach (var vein in m_veinDict.Values.SelectMany(v => v))
			{
				double totalWeight = vein.Ores.Sum(wb => wb.Weight);

				foreach (var wb in vein.Ores)
				{
					wb.WeightPercent = (wb.Weight / totalWeight) * 100.0;
				}
			}
		}

		private string GetFieldGuideOutputDirectory(string locale)
		{
			return Path.Combine(m_arguments.ModpackFolder, "kubejs/assets/tfc/patchouli_books/field_guide", locale, "entries/tfg_ores");
		}

		private PatchouliEntry GenerateOreIndexForDimension(string locale, Dimension dim, LocalizationTokens tokens)
		{
			var entry = new PatchouliEntry()
			{
				FileNameWithoutExtension = $"{dim.ID}_ore_index",
				Icon = dim.OreIndexIcon,
				Name = $"{dim.Translations[locale]} {tokens["ore_index"]}",
				ReadByDefault = true,
				Pages = []
			};

			// First page

			int numPages = 0;

			var pageBuilder = new PatchouliStringBuilder(new StringBuilder());
			pageBuilder.Append(string.Format(tokens["ore_index_format"], dim.Translations[locale]));

			entry.Pages.Add(new TextPage()
			{
				Title = $"{dim.Translations[locale]} {tokens["ore_index"]}",
				Text = pageBuilder.Dump(),
			});
			numPages++;

			// Build the list of veins, sorted alphabetically, with each vein sorted by densest

			var sortedVeins = m_oreDict.Values.Select(ore =>
				(ore,
				m_veinDict[dim]
					.Where(vein =>
						vein.Ores
							.Any(wb => wb.OreID == ore.ID))
							.OrderBy(vein =>
								vein.Ores.Single(wb => wb.OreID == ore.ID).Weight)))
				.OrderBy(tuple => tuple.ore.TranslatedNames[locale]);

			// Build the pages

			foreach (var chunk in sortedVeins.Chunk(14))
			{
				foreach ((var ore, var veins) in chunk)
				{
					pageBuilder.Append("$(li)");
					pageBuilder.Append($"{ore.TranslatedNames[locale]}: ");

					foreach (var vein in veins)
					{
						var wb = vein.Ores.Single(wb => wb.OreID == ore.ID);
						pageBuilder.InternalLink($"{(int) wb.WeightPercent!}%", $"tfg_ores/{dim.ID}_vein_index", vein.ID, true);

						if (vein != veins.Last())
						{
							pageBuilder.Append(", ");
						}
					}

					pageBuilder.Append(PatchouliStringBuilder.EMPTY);
				}

				entry.Pages.Add(new TextPage()
				{
					Text = pageBuilder.Dump()
				});
				numPages++;
			}

			return entry;
		}

		private PatchouliEntry GenerateVeinIndexForDimension(string locale, Dimension dim, LocalizationTokens tokens)
		{
			var entry = new PatchouliEntry()
			{
				FileNameWithoutExtension = $"{dim.ID}_vein_index",
				Icon = dim.VeinIndexIcon,
				Name = $"{dim.Translations[locale]} {tokens["vein_index"]}",
				ReadByDefault = true,
				Pages = []
			};

			// First page

			int numPages = 0;

			var pageBuilder = new PatchouliStringBuilder(new StringBuilder());
			pageBuilder.Append(string.Format(tokens["vein_index_format"], dim.Translations[locale]));

			entry.Pages.Add(new TextPage()
			{
				Title = $"{dim.Translations[locale]} {tokens["vein_index"]}",
				Text = pageBuilder.Dump()
			});
			numPages++;

			// Index pages

			foreach (var chunk in m_veinDict[dim].OrderBy(v => v.TranslatedNames[locale]).Chunk(14))
			{
				foreach (var vein in chunk)
				{
					pageBuilder.Append("$(li)");
					pageBuilder.InternalLink(vein.TranslatedNames[locale], $"tfg_ores/{dim.ID}_vein_index", vein.ID);
					pageBuilder.Append(PatchouliStringBuilder.EMPTY);
				}

				entry.Pages.Add(new TextPage()
				{
					Text = pageBuilder.Dump()
				});
				numPages++;
			}

			// If there's an odd number of pages, add a blank one

			if (numPages % 2 != 0)
			{
				entry.Pages.Add(new EmptyPage());
			}

			// Vein pages

			foreach (var vein in m_veinDict[dim].OrderBy(v => v.TranslatedNames[locale]))
			{
				numPages = 0;

				// Vein info page

				pageBuilder.ThingMacro(tokens["rarity"]);
				pageBuilder.Append($": {vein.Config.Rarity}");
				pageBuilder.LineBreak();

				pageBuilder.ThingMacro(tokens["density"]);
				pageBuilder.Append($": {vein.Config.Density}");
				pageBuilder.LineBreak();

				pageBuilder.ThingMacro(tokens["type"]);
				pageBuilder.Append($": {tokens[vein.Type]}");
				pageBuilder.LineBreak();

				pageBuilder.ThingMacro(tokens["y"]);
				pageBuilder.Append($": {vein.Config.MinY} — {vein.Config.MaxY}");
				pageBuilder.LineBreak();

				if (vein.Type == "tfc:disc_vein")
				{
					pageBuilder.ThingMacro(tokens["size"]);
					pageBuilder.Append($": {vein.Config.Size}");
					pageBuilder.LineBreak();

					pageBuilder.ThingMacro(tokens["height"]);
					pageBuilder.Append($": {vein.Config.Height}");
					pageBuilder.LineBreak();
				}
				else if (vein.Type == "tfc:cluster_vein")
				{
					pageBuilder.ThingMacro(tokens["size"]);
					pageBuilder.Append($": {vein.Config.Size}");
					pageBuilder.LineBreak();
				}
				else // pipe vein
				{
					pageBuilder.ThingMacro(tokens["height"]);
					pageBuilder.Append($": {vein.Config.Height}");
					pageBuilder.LineBreak();

					pageBuilder.ThingMacro(tokens["radius"]);
					pageBuilder.Append($": {vein.Config.Radius}");
					pageBuilder.LineBreak();
				}

				var indicator = vein.Indicator ?? IndicatorConfig.GenerateDefault(vein.Ores, m_oreDict);
				pageBuilder.ThingMacro(tokens["indicator_depth"]);
				pageBuilder.Append($": {indicator.Depth}");
				pageBuilder.LineBreak2();

				pageBuilder.ThingMacro(tokens["stone_types"]);
				pageBuilder.Append($": {string.Join(", ", vein.Rocks.Select(r => m_rockDict[r].Translations[locale]))}");

				if (vein.TranslatedInfo[locale] != null)
				{
					pageBuilder.LineBreak2();
					pageBuilder.Append(vein.TranslatedInfo[locale]!);
				}

				entry.Pages.Add(new TextPage
				{
					Title = vein.TranslatedNames[locale],
					Text = pageBuilder.Dump(),
					Anchor = vein.ID
				});
				numPages++;

				// One page per ore in the vein

				foreach (var block in vein.Ores.OrderBy(wb => wb.Weight))
				{
					var ore = m_oreDict[block.OreID];

					pageBuilder.ThingMacro(tokens["percentage"]);
					pageBuilder.Append($": {(int) block.WeightPercent!}%");

					if (ore.TranslatedInfo[locale] != null)
					{
						pageBuilder.LineBreak();
						pageBuilder.Append($"{ore.TranslatedInfo[locale]}");
					}

					if (ore.Formula != null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(tokens["formula"]);
						pageBuilder.Append($": {ore.Formula}");
					}

					if (ore.Hazard != null)
					{
						pageBuilder.LineBreak();
						pageBuilder.ThingMacro(tokens["hazard"]);
						pageBuilder.Append(": ");
						pageBuilder.Color(tokens[ore.Hazard], MinecraftColorCode.Red);
					}

					entry.Pages.Add(new MultiblockPage
					{
						Name = ore.TranslatedNames[locale],
						EnableVisualize = false,
						Text = pageBuilder.Dump(),
						Multiblock = ore.BuildMultiblockDisplay()
					});
					numPages++;
				}

				// If there's an odd number of pages, add a blank one

				if (numPages % 2 != 0)
				{
					entry.Pages.Add(new EmptyPage());
				}
			}

			return entry;
		}

		private void ExportConfiguredVeins()
		{
			string configuredFeatureDir = Path.Combine(m_arguments.ModpackFolder, "kubejs/data/tfg/worldgen/configured_feature");

			foreach ((var dimension, var veins) in m_veinDict)
			{
				string configuredVeinDir = Path.Combine(configuredFeatureDir, dimension.ID, "vein");

				// TODO: Clear directory

				foreach (var vein in veins)
				{
					var feature = new VeinConfiguredFeature(vein, m_rockDict, m_oreDict);
					string json = JsonSerializer.Serialize(feature, m_jsonOptions);
					File.WriteAllText(Path.Combine(configuredVeinDir, vein.ID + ".json"), json);
					ConsoleLogHelper.WriteLine($"Wrote out configured feature {vein.ID}", LogLevel.Info);
				}
			}
		}

		private void ExportPlacedVeins()
		{
			string placedFeatureDir = Path.Combine(m_arguments.ModpackFolder, "kubejs/data/tfg/worldgen/placed_feature");

			foreach ((var dimension, var veins) in m_veinDict)
			{
				string placedVeinDir = Path.Combine(placedFeatureDir, dimension.ID, "vein");

				// TODO: Clear directory

				foreach (var vein in veins)
				{
					// These are pretty bare-bones because tfc handles all the placement for you already
					var feature = new VeinPlacedFeature()
					{
						Feature = $"tfg:{dimension.ID}/vein/{vein.ID}",
					};

					string json = JsonSerializer.Serialize(feature, m_jsonOptions);
					File.WriteAllText(Path.Combine(placedVeinDir, vein.ID + ".json"), json);
					ConsoleLogHelper.WriteLine($"Wrote out placed feature {vein.ID}", LogLevel.Info);
				}
			}
		}

		private void ExportPatchouliEntries()
		{
			foreach (string locale in s_locales)
			{
				// Clear out existing files
				var outputDir = GetFieldGuideOutputDirectory(locale);
				//foreach (var existingPath in Directory.EnumerateFiles(outputDir))
				//{
				//	if (!m_arguments.WhitelistedPatchouliEntryFilenames.Contains(Path.GetFileNameWithoutExtension(existingPath)))
				//	{
				//		File.Delete(existingPath);
				//	}
				//}

				// Then write new ones
				foreach (var dimension in m_dimensionDict.Values)
				{
					var veinIndex = GenerateVeinIndexForDimension(locale, dimension, m_localeToTokens[locale]);

					string veinJson = JsonSerializer.Serialize(veinIndex, m_jsonOptions);
					File.WriteAllText(Path.Combine(outputDir, veinIndex.FileNameWithoutExtension + ".json"), veinJson);
					ConsoleLogHelper.WriteLine($"Wrote out {locale} {veinIndex.FileNameWithoutExtension}", LogLevel.Info);

					var oreIndex = GenerateOreIndexForDimension(locale, dimension, m_localeToTokens[locale]);

					string oreJson = JsonSerializer.Serialize(oreIndex, m_jsonOptions);
					File.WriteAllText(Path.Combine(outputDir, oreIndex.FileNameWithoutExtension + ".json"), oreJson);
					ConsoleLogHelper.WriteLine($"Wrote out {locale} {oreIndex.FileNameWithoutExtension}", LogLevel.Info);
				}
			}
		}
	}
}
