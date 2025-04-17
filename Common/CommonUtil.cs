namespace Common
{
    public static class CommonUtil
    {
        public const string MINECRAFT = "minecraft";
        public const string REPONAME = "Modpack-Modern";
		public const string KUBEJS = "kubejs";
		public const string ASSETS = "assets";

		public static DirectoryInfo GetModpackDirectory(string workingDir)
        {
            var toolsIndex = workingDir.IndexOf("Tools-Modern");
            if (toolsIndex == -1)
            {
                throw new DirectoryNotFoundException("Failed to find \"Tools-Modern\" directory.");
            }

            var teamDir = workingDir.Substring(0, toolsIndex);

            var modpackDir = teamDir + "Modpack-Modern";

            return new DirectoryInfo(modpackDir);
        }

		public static DirectoryInfo GetKJSAssetsFolder(DirectoryInfo modpackFolder)
		{
			string kjsAssetsFolder = Path.Combine(modpackFolder.FullName, KUBEJS, ASSETS);
			if (!Directory.Exists(kjsAssetsFolder))
			{
				throw new DirectoryNotFoundException($"The \"{ASSETS}\" folder was not found in {kjsAssetsFolder}");
			}
			return new DirectoryInfo(kjsAssetsFolder);
		}
	}
}
