namespace OresToFieldGuide
{
    public class OreIndexEntry(string ore)
    {
        public required string Ore { get; set; } = ore;

        public Dictionary<string, float> VeinToWeight { get; private set; } = [];

        public Dictionary<string, int> VeinToCount { get; private set; } = [];

        public void SortVeinsByRichestWeight()
        {
            if (VeinToCount == null)
                return;

            if (VeinToWeight == null)
                return;

            VeinToWeight = VeinToWeight.OrderByDescending(kvp => kvp.Value).ToDictionary();
            Dictionary<string, int> orderedVeinToCount = new Dictionary<string, int>();

            foreach(var veinName in VeinToWeight.Keys)
            {
                orderedVeinToCount[veinName] = VeinToCount[veinName];
            }
            VeinToCount = orderedVeinToCount;
        }

        internal Multiblock BuildMultiblockDisplay()
        {
            return new Multiblock()
            {
                Mapping = new Dictionary<string, string>
                {
                    ["0"] = $"#forge:ores/{Ore}"
                },
                Pattern = [
                    ["0"],
                    [" "]
                ]
            };
        }

        internal bool TryGetNormalizedWeightInVein(Vein vein, out int normalizedWeight)
        {
            return TryGetNormalizedWeightInVein(vein.FileName, out normalizedWeight);
        }

        internal bool TryGetNormalizedWeightInVein(string vein, out int normalizedWeight)
        {
            normalizedWeight = 0;
            if (!VeinToCount.TryGetValue(vein, out var count))
            {
                return false;
            }
            if (!VeinToWeight.TryGetValue(vein, out var weight))
            {
                return false;
            }
            normalizedWeight = (int)(weight / count);
            return true;
        }
    }
}
