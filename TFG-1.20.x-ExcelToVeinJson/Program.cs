using System.Globalization;
using Newtonsoft.Json;
using OfficeOpenXml;
using TFG_1._20.x_ExcelToVeinJson.Models;

namespace TFG_1._20.x_ExcelToVeinJson
{
    internal class Program
    {
        private const string PathToExcel = "../../../../file.xlsx";
        private const string PathToConfiguredFeature = "../../../../Jsons/configured_feature/vein/";
        private const string PathToPlacedFeature = "../../../../Jsons/placed_feature/vein/";
        private const string PathToVeinList = "../../../../Jsons/";
        
        private const int VeinNameColumnId = 1;
        private const int FirstOreColumnId = 2;
        private const int SecondOreColumnId = 3;
        private const int ThirdOreColumnId = 4;
        private const int FourthOreColumnId = 5;
        private const int FifthsOreColumnId = 6;
        private const int MinYColumnId = 7;
        private const int MaxYColumnId = 8;
        private const int DensityColumnId = 9;
        private const int SizeColumnId = 10;
        private const int RarityColumnId = 11;
        private const int IndicatorMaterialColumnId = 12;
        private const int IndicatorRarityColumnId = 13;
        private const int IndicatorDepthColumnId = 14;
        private const int IndicatorURColumnId = 15;
        private const int IndicatorUCColumnId = 16;
        private const int IndicatorMinSkewColumnId = 17;
        private const int IndicatorMaxSkewColumnId = 18;
        private const int IndicatorMinSlantColumnId = 19;
        private const int IndicatorMaxSlantColumnId = 20;
        private const int IndicatorSignColumnId = 21;
        private const int IndicatorHeightColumnId = 22;
        private const int IndicatorRadiusColumnId = 23;
        private const int IndicatorBiomeListColumnId = 24;
        private const int IndicatorProjectColumnId = 25;
        private const int IndicatorProjectOffsetColumnId = 26;
        private const int IndicatorVeinTypeColumnId = 27;
        private const int IEColumnId = 28;
        private const int IIColumnId = 29;
        private const int MColumnId = 30;
        private const int SColumnId = 31;
        private const int CSColumnId = 32;
        
        private static HashSet<string> _ieStoneTypeNames = new()
        {
            "rhyolite",
            "basalt",
            "andesite",
            "dacite"
        };
        
        private static HashSet<string> _iiStoneTypeNames = new()
        {
            "granite",
            "diorite",
            "gabbro"
        };
        
        private static HashSet<string> _metamorphicStoneTypeNames = new()
        {
            "quartzite",
            "slate",
            "phyllite",
            "schist",
            "gneiss",
            "marble"
        };
        
        private static HashSet<string> _sedimentaryStoneTypeNames = new()
        {
            "shale",
            "claystone",
            "limestone",
            "conglomerate",
            "dolomite",
            "chert",
            "chalk"
        };
        
        private static void Main(string[] args)
        {
            var configuredFeatureDirectory = new DirectoryInfo(PathToConfiguredFeature);
            foreach (var file in configuredFeatureDirectory.GetFiles()) file.Delete();

            var placedFeatureDirectory = new DirectoryInfo(PathToPlacedFeature);
            foreach (var file in placedFeatureDirectory.GetFiles()) file.Delete();
            
            const string pathToExcel = PathToExcel;
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(pathToExcel);
            var sheet = package.Workbook.Worksheets["OreGenTable"];

            var veinNameList = new List<string>()
            {
                "tfc:vein/gravel",
                "tfc:vein/kaolin_disc",
                "tfc:vein/granite_dike",
                "tfc:vein/diorite_dike",
                "tfc:vein/gabbro_dike",
                "tfc:geode"
            };
            
            for (var row = 3; row < 999; row++)
            {
                dynamic veinName = sheet.Cells[row, VeinNameColumnId].Value;
                if (veinName == null) break;
                veinName = veinName.ToString()!.ToLower().Replace(' ', '_');
                
                var firstOre = ConvertOreNameWeightToTuple(sheet.Cells[row, FirstOreColumnId].Value);
                var secondOre = ConvertOreNameWeightToTuple(sheet.Cells[row, SecondOreColumnId].Value);
                var thirdOre = ConvertOreNameWeightToTuple(sheet.Cells[row, ThirdOreColumnId].Value);
                var fouthOre = ConvertOreNameWeightToTuple(sheet.Cells[row, FourthOreColumnId].Value);
                var fifthOre = ConvertOreNameWeightToTuple(sheet.Cells[row, FifthsOreColumnId].Value);
                
                var minHeight = int.Parse(sheet.Cells[row, MinYColumnId].Value.ToString()!);
                var maxHeight = int.Parse(sheet.Cells[row, MaxYColumnId].Value.ToString()!);
                var density = float.Parse(sheet.Cells[row, DensityColumnId].Value.ToString()!, CultureInfo.InvariantCulture);
                var size = int.Parse(sheet.Cells[row, SizeColumnId].Value.ToString()!);
                var rarity = int.Parse(sheet.Cells[row, RarityColumnId].Value.ToString()!);
                string? indicator = null; /*sheet.Cells[row, IndicatorMaterialColumnId].Value != null ? sheet.Cells[row, 12].Value.ToString() : null;*/
                
                int? indicatorRarity = null;
                int? indicatorDepth = null;
                int? indicatorUR = null;
                int? indicatorUC = null;

                if (indicator != null)
                {
                    indicatorRarity = sheet.Cells[row, IndicatorRarityColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorRarityColumnId].Value.ToString()!) : null;
                    indicatorDepth = sheet.Cells[row, IndicatorDepthColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorDepthColumnId].Value.ToString()!) : null;
                    indicatorUR = sheet.Cells[row, IndicatorURColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorURColumnId].Value.ToString()!) : null;
                    indicatorUC = sheet.Cells[row, IndicatorUCColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorUCColumnId].Value.ToString()!) : null;
                }
                
                int? minSkew = sheet.Cells[row, IndicatorMinSkewColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorMinSkewColumnId].Value.ToString()!) : null;
                int? maxSkew = sheet.Cells[row, IndicatorMaxSkewColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorMaxSkewColumnId].Value.ToString()!) : null;
                int? minSlant = sheet.Cells[row, IndicatorMinSlantColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorMinSlantColumnId].Value.ToString()!) : null;
                int? maxSlant = sheet.Cells[row, IndicatorMaxSlantColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorMaxSlantColumnId].Value.ToString()!) : null;
                int? sign = sheet.Cells[row, IndicatorSignColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorSignColumnId].Value.ToString()!) : null;
                int? height = sheet.Cells[row, IndicatorHeightColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorHeightColumnId].Value.ToString()!) : null;
                int? radius = sheet.Cells[row, IndicatorRadiusColumnId].Value != null ? int.Parse(sheet.Cells[row, IndicatorRadiusColumnId].Value.ToString()!) : null;
                var biomeTag = sheet.Cells[row, IndicatorBiomeListColumnId].Value != null ? sheet.Cells[row, IndicatorBiomeListColumnId].Value.ToString() : null;
                bool? project = sheet.Cells[row, IndicatorProjectColumnId].Value != null ? bool.Parse(sheet.Cells[row, IndicatorProjectColumnId].Value.ToString()!) : null;
                bool? projectOffset = sheet.Cells[row, IndicatorProjectOffsetColumnId].Value != null ? bool.Parse(sheet.Cells[row, IndicatorProjectOffsetColumnId].Value.ToString()!) : null;
                var veinType = sheet.Cells[row, IndicatorVeinTypeColumnId].Value.ToString()!;
                
                var spawnsInIe = sheet.Cells[row, IEColumnId].Value != null ? _ieStoneTypeNames : null;
                var spawnsInIi = sheet.Cells[row, IIColumnId].Value != null ? _iiStoneTypeNames : null;
                var spawnsInM = sheet.Cells[row, MColumnId].Value != null ? _metamorphicStoneTypeNames : null;
                var spawnsInS = sheet.Cells[row, SColumnId].Value != null ? _sedimentaryStoneTypeNames : null;
                var spawnsInCustom = sheet.Cells[row, CSColumnId].Value != null 
                    ? new HashSet<string>(sheet.Cells[row, CSColumnId].Value!.ToString()!.ToLower().Split(", ")) : null;
                
                var oresArray = new[] { firstOre, secondOre, thirdOre, fouthOre, fifthOre };
                var blockSpawnInArray = new[] { spawnsInIe, spawnsInIi, spawnsInM, spawnsInS, spawnsInCustom };

                var oreBlocksList = new List<OreBlocks>();
                
                foreach (var setWithRockCategories in blockSpawnInArray)
                {
                    if (setWithRockCategories == null) continue;
                    
                    foreach (var stoneTypeName in setWithRockCategories)
                    {
                        var oreList = new List<BlockEntry>();
                        
                        foreach (var oreMaterialWithWidth in oresArray)
                        {
                            if (oreMaterialWithWidth == null) continue;

                            oreList.Add(new BlockEntry
                            {
                                Block = $"gtceu:tfc_{stoneTypeName}_{oreMaterialWithWidth.Item1}_ore",
                                Weight = oreMaterialWithWidth!.Item2
                            });
                        }
                        
                        oreBlocksList.Add(new OreBlocks
                        {
                            Replace = new[]
                            {
                                $"tfc:rock/raw/{stoneTypeName}"
                            },
                            With = oreList.ToArray()
                        });
                    }
                }
                
                var veinObject = new VeinModel
                {
                    Type = veinType,
                    Config = new Config
                    {
                        Rarity = rarity,
                        Density = density,
                        MinY = minHeight,
                        MaxY = maxHeight,
                        Size = size,
                        RandomName = veinName,
                        MinSkew = minSkew,
                        MaxSkew = maxSkew,
                        MinSlant = minSlant,
                        MaxSlant = maxSlant,
                        Sign = sign,
                        Height = height,
                        Radius = radius,
                        Project = project,
                        ProjectOffset = projectOffset,
                        Biomes = biomeTag,
                        Blocks = oreBlocksList.ToArray(),
                        Indicator = indicator != null ? new Indicator
                            {
                                Rarity = indicatorRarity,
                                Depth = indicatorDepth,
                                UndergroundRarity = indicatorUR,
                                UndergroundCount = indicatorUC,
                                Blocks = new[]
                                {
                                    new BlockEntry
                                    {
                                        Block = "null",
                                        Weight = 100
                                    }
                                }
                            }
                            : null,
                    }
                };

                veinNameList.Add(veinName);
                
                
                var configuredFeatureJson = JsonConvert.SerializeObject(veinObject, Formatting.Indented);
                File.WriteAllText($"{PathToConfiguredFeature}{veinName}.json", configuredFeatureJson);

                var placedObject = new PlacedFeatureModel
                {
                    Feature = $"terrafirmagreg:vein/{veinName}"
                };
                
                var placedFeatureJson = JsonConvert.SerializeObject(placedObject, Formatting.Indented);
                File.WriteAllText($"{PathToPlacedFeature}{veinName}.json", placedFeatureJson);
                
                Console.WriteLine($"Complete: {veinName}");
            }

            var veinListObject = new VeinListModel
            {
                Values = veinNameList.Select(el => $"terrafirmagreg:vein/{el}").ToArray()
            };

            var veinListJson = JsonConvert.SerializeObject(veinListObject, Formatting.Indented);
            File.WriteAllText($"{PathToVeinList}veins.json", veinListJson);
            
            Console.WriteLine("Complete!");
        }

        private static Tuple<string, int>? ConvertOreNameWeightToTuple(object? value)
        {
            if (value == null) return null;
            
            var stringValue = value.ToString()!;
            var array = stringValue.Split(' ');

            var materialName = array[0].ToSnakeCase();
            var materialWeight = int.Parse(
                array[1].Replace("(", string.Empty).Replace(")", string.Empty)
            );

            return new Tuple<string, int>(materialName, materialWeight);
        }
    }
}