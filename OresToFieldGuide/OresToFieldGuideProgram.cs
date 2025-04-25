using System.Text;
using Common;

namespace OresToFieldGuide
{
    internal class OresToFieldGuideProgram
    {
        public static string s_fallbackLocale => s_locales[0];
        public static readonly string[] s_locales = 
        [
            "en_us", //US English
            "it_it", //Italian
            "ru_ru", //Russian
            "uk_ua" //Ukranian
        ];

        private ProgramArguments m_arguments;
        private Dictionary<string, MineralData> m_localeToMineralData = new Dictionary<string, MineralData>();
        private Dictionary<string, LocalizationTokens> m_localeToTokens = new Dictionary<string, LocalizationTokens>();
        private Dictionary<string, Output> m_localeToOutputs = new Dictionary<string, Output>();
        private Dictionary<string, Vein[]> m_dimensionsToVeins = new Dictionary<string, Vein[]>();

        public bool Run()
        {
            DeserializeLanguageTokens();
            DeserializeMineralDatas();
            DeserializeVeins();
            CreateOutputDirectories();

            foreach(string locale in s_locales)
            {
                if(!m_localeToOutputs.TryGetValue(locale, out Output output))
                {
                    ConsoleLogHelper.WriteLine($"Not running the program against locale {locale}. There is no valid Output struct for it.", LogLevel.Error);
                    continue;
                }
                
                if(!m_localeToMineralData.TryGetValue(locale, out MineralData mineralData))
                {
                    ConsoleLogHelper.WriteLine($"Not running the program against locale {locale}. There is no valid MineralData for it.", LogLevel.Error);
                    continue;
                }

                if(!m_localeToTokens.TryGetValue(locale, out LocalizationTokens localizationTokens))
                {
                    ConsoleLogHelper.WriteLine($"Not running the program against locale {locale}. There is no valid LocalizationTokens for it.", LogLevel.Error);
                    continue;
                }
                var tokens = m_localeToTokens[locale];

                CreateEntriesForLocale(tokens, mineralData, output);
            }

            WriteFiles();

            if(m_arguments.shouldOverwriteFiles)
                MoveFilesToInGameFieldGuide();

            return true;
        }

        private void DeserializeLanguageTokens()
        {
            var en_usTokenPath = Path.Combine(m_arguments.languageTokenFolder, $"{s_fallbackLocale}.json");
            LocalizationTokens en_usTokens = LocalizationTokens.FromJSON(en_usTokenPath);

            m_localeToTokens.Add(s_fallbackLocale, en_usTokens);

            foreach (var locale in s_locales)
            {
                if (m_localeToTokens.ContainsKey(locale))
                    continue;

                var localeTokenPath = Path.Combine(m_arguments.languageTokenFolder, $"{locale}.json");
                if (!File.Exists(localeTokenPath))
                {
                    ConsoleLogHelper.WriteLine($"Could not file localization tokens for locale {locale}! Assigning {s_fallbackLocale}'s Localization Tokens.", LogLevel.Warning);
                    m_localeToTokens.Add(locale, en_usTokens);
                    continue;
                }

                LocalizationTokens localeTokens = LocalizationTokens.FromJSON(localeTokenPath);

                m_localeToTokens.Add(locale, localeTokens);
            }
        }

        private void DeserializeMineralDatas()
        {
            var mineralDatas = Directory.EnumerateFiles(m_arguments.mineralDataFolder);

            foreach(var mineralDataJSON in mineralDatas)
            {
                var languageName = Path.GetFileNameWithoutExtension(mineralDataJSON);
                var mineralData = MineralData.FromJSON(mineralDataJSON);
                m_localeToMineralData.Add(languageName, mineralData);
            }
        }

        /// <summary>
        /// Deserializes all the veins and stores them in a dictionary of planet to veins
        /// </summary>
        private void DeserializeVeins()
        {
            foreach(var planet in m_arguments.planetToVeinsPath.Keys)
            {
                var serializedVeins = m_arguments.planetToVeinsPath[planet];
                Vein[] deserializedVeins = new Vein[serializedVeins.Length];

                for (int i = 0; i < serializedVeins.Length; i++)
                {
                    string veinJSON = serializedVeins[i];
                    Vein vein = Vein.FromJSON(veinJSON);
                    deserializedVeins[i] = vein;
                }

                m_dimensionsToVeins.Add(planet, deserializedVeins);
            }
        }

        /// <summary>
        /// Creates the output directories for the program, which includes the root output folder, the locale's root folder inside the field guide, and it's pages output
        /// </summary>
        private Task CreateOutputDirectories()
        {
            var workingDirectory = Directory.GetCurrentDirectory();
            var outputDirectory = Path.Combine(workingDirectory, "OUTPUT");

            Directory.CreateDirectory(outputDirectory);

            foreach(var local in s_locales)
            {
                string fieldGuideOutput = Path.Combine(outputDirectory, MainClass.KUBEJS, MainClass.ASSETS, MainClass.TFC, MainClass.PATCHOULI_BOOKS, MainClass.FIELD_GUIDE, local);
                Directory.CreateDirectory(fieldGuideOutput);

                string pagesOutput = Path.Combine(fieldGuideOutput, MainClass.ENTRIES, MainClass.TFG_ORES);
                Directory.CreateDirectory(pagesOutput);

                m_localeToOutputs.Add(local, new Output
                {
                    rootOutputFolder = outputDirectory,
                    fieldGuideOutput = fieldGuideOutput,
                    pagesOutput = pagesOutput,
                    pages = new List<PatchouliEntry>()
                });
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the actual patchouli entries, 2 entries will be generated per planet found in <see cref="m_dimensionsToVeins"/>.
        /// <list type="bullet">
        /// <item>planet_vein_index.json</item>
        /// <item>planet_ore_index.json</item>
        /// </list>
        /// </summary>
        /// <returns>A Dictionary of Planet Name to Patchouli Entry</returns>
        private void CreateEntriesForLocale(LocalizationTokens tokens, MineralData mineralData, Output localizationOutput)
        {
            foreach(var planet in m_dimensionsToVeins.Keys)
            {
                localizationOutput.pages ??= new List<PatchouliEntry>();
                Dictionary<string, OreIndexEntry> oreToOreIndexEntry = new Dictionary<string, OreIndexEntry>();

                localizationOutput.pages.Add(GenerateOreIndexForPlanet(planet, tokens, mineralData, m_dimensionsToVeins[planet], oreToOreIndexEntry));
                localizationOutput.pages.Add(GenerateVeinIndexForPlanet(planet, tokens, mineralData, m_dimensionsToVeins[planet], oreToOreIndexEntry));
            }
        }

        private PatchouliEntry GenerateVeinIndexForPlanet(string planet, LocalizationTokens tokens, MineralData mineralData, Vein[] veins, Dictionary<string, OreIndexEntry> oreToEntry)
        {
            var entry = new PatchouliEntry()
            {
                FileNameWithoutExtension = $"{planet}_vein_index",
                Icon = "minecraft:stone",
                Name = $"{tokens.Dimensions[planet]} {tokens.Keywords["Vein Index"]}",
                ReadByDefault = true,
                Pages = []
            };

            PatchouliStringBuilder pageBuilder = new PatchouliStringBuilder(new StringBuilder());

            pageBuilder.Append(string.Format(tokens.VeinIndex, tokens.Dimensions[planet]));

            entry.Pages.Add(new TextPage()
            {
                Title = $"{tokens.Dimensions[planet]} {tokens.Keywords["Vein Index"]}",
                Text = pageBuilder.Dump(),
            });

            //Sort the veins alphabetically
            List<KeyValuePair<string, Vein>> veinNames = new List<KeyValuePair<string, Vein>>();
            foreach (var vein in veins)
            {
                veinNames.Add(new KeyValuePair<string, Vein>(vein.ComputeWeightiestOreNames(tokens.RockDictionary.Keys.ToArray(), mineralData, oreToEntry), vein));
            }

            veinNames = veinNames.OrderBy(x => x.Key).ToList();
            //Build index of veins
            TextPage veinIndexPage = new TextPage() { Text = "" };
            for (int i = 0; i < veinNames.Count; i++)
            {
                KeyValuePair<string, Vein> kvp = veinNames[i];
                var veinName = veinNames[i].Key;
                var vein = veinNames[i].Value;

                if (i != 0 && i % 14 == 0)
                {
                    veinIndexPage.Text = pageBuilder.Dump();
                    entry.Pages.Add(veinIndexPage);

                    veinIndexPage = new TextPage() { Text = "" };
                }

                pageBuilder.List($"$(l:tfg_ores/{entry.FileNameWithoutExtension}#{vein.FileName}){veinName}$()");
            }

            pageBuilder.Clear();
            foreach(var kvp in veinNames)
            {
                string veinName = kvp.Key;
                Vein vein = kvp.Value;

                Vein.VeinConfig config = vein.VeinConfig;
                //Build the main page
                TextPage mainPage = new TextPage()
                {
                    Text = "",
                    Anchor = $"{vein.FileName}",
                };
                // I'd like to make it so the main page for the name displayed a translated, nicified file name. CBA atm tho
                //mainPage.Title = vein.ComputeVeinTitle(tokens.KeywordDictionary, mineralData);

                //Rarity
                if(config.Rarity.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Rarity"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.Rarity}");
                    pageBuilder.LineBreak();
                }

                //Density
                if(config.Density.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Density"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.Density}");
                    pageBuilder.LineBreak();
                }

                //Type
                pageBuilder.ThingMacro(tokens.Keywords["Type"]);
                pageBuilder.Append(": ");
                if (Util.TryRemoveStartingSubstring(vein.Type, "tfc", out string withoutModID))
                {
                    if(Util.TryRemoveLastSubstring(withoutModID, "vein", out string withoutTypeSubstring))
                    {
                        pageBuilder.Append(tokens.VeinTypeDictionary[withoutTypeSubstring]);
                    }
                    else
                    {
                        pageBuilder.Color("FAILED TO PARSE", MinecraftColorCode.DarkRed);
                    }
                }
                else
                {
                    pageBuilder.Color("FAILED TO PARSE", MinecraftColorCode.DarkRed);
                }
                pageBuilder.LineBreak();

                //Y
                if(config.MinY.HasValue && config.MaxY.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Y"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.MinY} -> {config.MaxY}");
                    pageBuilder.LineBreak();
                }

                //Size
                if(config.Size.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Size"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.Size}");
                    pageBuilder.LineBreak();
                }

                //Height
                if (config.Height.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Height"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.Height}");
                    pageBuilder.LineBreak();
                }

                if(config.Radius.HasValue)
                {
                    pageBuilder.ThingMacro(tokens.Keywords["Radius"]);
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{config.Radius}");
                    pageBuilder.LineBreak();
                }

                pageBuilder.LineBreak();

                //Stone Types
                pageBuilder.ThingMacro(tokens.Keywords["Stone types"]);
                pageBuilder.Append(": ");
                string[] rocksInVein = vein.GetRocksInVein(tokens.RockDictionary.Keys.ToArray());
                for(int i = 0; i < rocksInVein.Length; i++)
                {
                    var rock = rocksInVein[i];
                    pageBuilder.Append($"{tokens.RockDictionary[rock]}");
                    if(i == rocksInVein.Length - 1)
                    {
                        pageBuilder.Append('.');
                    }
                    else
                    {
                        pageBuilder.Append(", ");
                    }
                }
                pageBuilder.LineBreak2();
                mainPage.Text = pageBuilder.Dump();

                entry.Pages.Add(mainPage);

                //Build the ore pages
                int orePageCount = 0;
                foreach(var oreName in oreToEntry.Keys)
                {
                    var oreIndexEntry = oreToEntry[oreName];
                    if(!oreIndexEntry.TryGetNormalizedWeightInVein(vein.FileName, out int normalizedWeight))
                    {
                        continue;
                    }

                    if(!Util.TryRemoveLastSubstring(oreName, "ore", out string orelessString))
                    {
                        continue;
                    }

                    //This ore is present in the current vein, log it.
                    MineralData.Entry? mineralDataEntry = mineralData.FindMineral(orelessString);
                    if(mineralDataEntry == null)
                    {
                        continue;
                    }

                    var multiblockPage = new MultiblockPage
                    {
                        Name = mineralDataEntry.Name,
                        Multiblock = oreIndexEntry.BuildMultiblockDisplay(),
                        EnableVisualize = false,
                    };

                    //Percentage
                    pageBuilder.ThingMacro($"{tokens.Keywords["Percentage"]}");
                    pageBuilder.Append(": ");
                    pageBuilder.Append($"{normalizedWeight}%");
                    pageBuilder.LineBreak();
                    
                    //Use and For
                    if(mineralDataEntry.Use != null && mineralDataEntry.For != null)
                    {
                        pageBuilder.ThingMacro($"{mineralDataEntry.Use}");
                        pageBuilder.Append(": ");
                        for (int i = 0; i < mineralDataEntry.For.Length; i++)
                        {
                            string? forUse = mineralDataEntry.For[i];
                            pageBuilder.Append(forUse);
                            if(i != mineralDataEntry.For.Length - 1)
                            {
                                pageBuilder.Append(", ");
                            }
                            else
                            {
                                pageBuilder.Append(".");
                            }
                        }
                        pageBuilder.LineBreak();
                    }

                    //Formula
                    if(mineralDataEntry.Formula != null)
                    {
                        pageBuilder.ThingMacro($"{tokens.Keywords["Formula"]}");
                        pageBuilder.Append(": ");
                        pageBuilder.Append($"{mineralDataEntry.Formula}");
                        pageBuilder.LineBreak();
                    }

                    //Is Hazardous
                    if(mineralDataEntry.Hazardous)
                    {
                        pageBuilder.Color($"[{tokens.Keywords["Hazardous"].ToUpperInvariant()}]", MinecraftColorCode.Red);
                    }

                    multiblockPage.Text = pageBuilder.Dump();
                    entry.Pages.Add(multiblockPage);
                    orePageCount++;
                }
                if(orePageCount % 2 == 0)
                {
                    entry.Pages.Add(new EmptyPage());
                }
            }

            return entry;
        }

        private Task<PatchouliEntry> GenerateOreIndexForPlanet(string planet, LocalizationTokens tokens, MineralData mineralData, Vein[] veins, Dictionary<string, OreIndexEntry> oreToEntry)
        {
            var patchouliEntry = new PatchouliEntry()
            {
                FileNameWithoutExtension = $"{planet}_ore_index",
                Icon = "minecraft:stone",
                Name = $"{tokens.Dimensions[planet]} {tokens.Keywords["Ore Index"]}",
                ReadByDefault = true
            };

            List<PatchouliPage> patchouliPages = new List<PatchouliPage>();

            PatchouliStringBuilder pageBuilder = new PatchouliStringBuilder(new StringBuilder());

            pageBuilder.Append(string.Format(tokens.OreIndex, tokens.Dimensions[planet]));

            patchouliPages.Add(new TextPage()
            {
                Title = $"{tokens.Dimensions[planet]} {tokens.Keywords["Ore Index"]}",
                Text = pageBuilder.Dump()
            });

            patchouliPages.Add(new EmptyPage());

            //Iterate thru all the veins of the planet
            foreach(var vein in veins)
            {
                //Try to get the vein's ores and their percentages.
                if(!vein.TryGetOresAndPercentage(out var weightedOres))
                {
                    continue;
                }

                //Iterate thru the weighted ores within the veins, most of the veins may have ores that are within certain types of rocks.
                foreach (var weightedOre in weightedOres)
                {
                    //Find this weighted ore's actual ore (IE: If its granite_redstone, this will return just "redstone"
                    string rockless = "";
                    foreach (var rock in tokens.RockDictionary.Keys)
                    {
                        if (weightedOre.TryGetOreNameWithoutIDAndWithoutRock(rock, out rockless))
                        {
                            break;
                        }
                    }

                    //Are we already storing this ore's entry? if not, create a new entry.
                    if (!oreToEntry.TryGetValue(rockless, out OreIndexEntry oreIndexEntry))
                    {
                        oreIndexEntry = new OreIndexEntry
                        {
                            Ore = rockless
                        };
                    }

                    //Increment the total weight of the ore within its vein
                    if (!oreIndexEntry.VeinToWeight.TryGetValue(vein.FileName, out _))
                    {
                        oreIndexEntry.VeinToWeight[vein.FileName] = 0;
                    }
                    oreIndexEntry.VeinToWeight[vein.FileName] += weightedOre.weight;

                    //Increment the total amount of times we've seen this ore in the vein.
                    if (!oreIndexEntry.VeinToCount.TryGetValue(vein.FileName, out _))
                    {
                        oreIndexEntry.VeinToCount[vein.FileName] = 0;
                    }
                    oreIndexEntry.VeinToCount[vein.FileName]++;

                    oreIndexEntry.SortVeinsByRichestWeight();
                    //Update the ore index entry
                    oreToEntry[rockless] = oreIndexEntry;
                }
            }

            var alphabetizedOres = oreToEntry.Keys.OrderBy(k => k).ToArray();
            TextPage oreIndexPage = new TextPage() { Text = "" };

            for(int i = 0; i < alphabetizedOres.Length; i++)
            {
                if (i != 0 && i % 14 == 0)
                {
                    oreIndexPage.Text = pageBuilder.Dump();
                    patchouliPages.Add(oreIndexPage);

                    oreIndexPage = new TextPage() { Text = "" };
                }

                var ore = alphabetizedOres[i];
                var oreIndexEntry = oreToEntry[ore];

                if(!Util.TryRemoveLastSubstring(ore, "ore", out var mineral))
                {
                    continue;
                }

                MineralData.Entry? mineralDataEntry = mineralData.FindMineral(mineral);
                if (mineralDataEntry == null)
                    continue;

                pageBuilder.ThingMacro($"{mineralDataEntry.Name}");
                pageBuilder.Append(": ");
                foreach (var veinName in oreIndexEntry.VeinToWeight.Keys)
                {
                    if(!oreIndexEntry.TryGetNormalizedWeightInVein(veinName, out var normalizedWeight))
                    {
                        continue;
                    }
                    pageBuilder.InternalLink($"{normalizedWeight}%", $"tfg_ores/{planet}_vein_index", veinName);
                    pageBuilder.Append(" ");
                }
                pageBuilder.LineBreak();
            }

            patchouliEntry.Pages = patchouliPages.ToArray();

            var textPages = patchouliEntry.Pages.OfType<TextPage>();

            return Task.FromResult(patchouliEntry);
        }

        private async Task WriteFiles()
        {
            //Iterate thru the locales
            foreach(var locale in m_localeToOutputs.Keys)
            {
                Output output = m_localeToOutputs[locale];

                //Delete the previous outputs
                foreach(var filePath in Directory.EnumerateFiles(output.pagesOutput))
                {
                    File.Delete(filePath);
                }

                //write the new outputs
                foreach(var page in output.pages)
                {
                    var filePath = Path.Combine(output.pagesOutput, page.FileNameWithoutExtension + ".json");

                    using (StreamWriter writer = File.CreateText(filePath))
                    {
                        var json = JsonConvert.SerializeObject(page, m_arguments.shouldPrettyPrint ? Formatting.Indented : Formatting.None, new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                        });
                        await writer.WriteAsync(json);
                        ConsoleLogHelper.WriteLine($"Wrote file {page.FileNameWithoutExtension} and saved it in {filePath}", LogLevel.Info);
                    }
                }
            }
        }

        private Task MoveFilesToInGameFieldGuide()
        {
            //Iterate thru the locales
            foreach(var locale in m_localeToOutputs.Keys)
            {
                Output output = m_localeToOutputs[locale];

                //Get the path from the KubeJS folder down to this locale's tfg_ores path, something along the lines of "\kubejs\assets\tfc\patchouli_books\field_guide\en_us\entries\tfg_ores" for english
                string kubeJSFolderToTFGOresFolderPath = output.pagesOutput.Substring(output.rootOutputFolder.Length + 1); //+2 because this will capute the starting "\" in the variable, there has to be a cleaner way but idgaf
                string destinationFolder = Path.Combine(m_arguments.minecraftFolder, kubeJSFolderToTFGOresFolderPath);

                var filePathsInDestinationFolder = Directory.EnumerateFiles(destinationFolder);
                foreach(var existingFilePath in filePathsInDestinationFolder)
                {
                    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(existingFilePath);

                    //If the file name points to one of the manually written patchouli entries, do not delete it.
                    if(m_arguments.whitelistedPatchouliEntryFilenames.Contains(fileNameWithoutExtension))
                    {
                        continue;
                    }
                    File.Delete(existingFilePath);
                }

                var outputPages = Directory.EnumerateFiles(output.pagesOutput);
                foreach (var pagePath in outputPages)
                {
                    string fileName = Path.GetFileName(pagePath);
                    File.Copy(pagePath, Path.Combine(destinationFolder, fileName), true);
                }
            }
            return Task.CompletedTask;
        }

        public OresToFieldGuideProgram(ProgramArguments arguments)
        {
            m_arguments = arguments;
        }

        internal struct Output
        {
            public string rootOutputFolder;
            public string fieldGuideOutput;
            public string pagesOutput;

            public List<PatchouliEntry> pages;
        }
    }
}
