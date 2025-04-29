﻿//Uncomment this to enable verification of vein weights
#define VERIFY_VEIN_WEIGHTS
//Uncomment this to write json in a Prettified format, only really useful for debugging the output
#define PRETTY_PRINT
//Uncomment this to make the program overwrite all the files within .minecraf/kubejs/assets/../tfg_ores/
#define OVERWRITE_FILES

using Common;
using System.Collections.Generic;
using System.Text;

namespace OresToFieldGuide
{
	/// <summary>
	/// Main class of the program. Contains the entry method alongside the creation of arguments and passing these arguments to an instance of <see cref="OresToFieldGuideProgram"/>
	/// </summary>
	public class MainClass
	{
		public const string PROJECT_NAME = "OresToFieldGuide";

		/// <summary>
		/// Main method for the Program
		/// </summary>
		public static void Main(string[] args)
		{
			Console.WriteLine("Creating updated entries of Ores for the Field Guide!");

			//Get our arguments based off the application's location, preprocessor directives, etc
			if (!TryGetProgramArguments(out ProgramArguments programArguments))
			{
				ConsoleLogHelper.WriteLine("Failed to get Program's Arguments, Press any key to exit...", LogLevel.Error);
				Console.ReadKey();
				return;
			}

			ConsoleLogHelper.WriteLine("Arguments have been obtained! Printing...", LogLevel.Info);

			ConsoleLogHelper.WriteLine($".minecraft Folder Path: \"{programArguments.ModpackFolder}\"", LogLevel.Info);
			ConsoleLogHelper.WriteLine($"data Folder Path: \"{programArguments.DataFolder}\"", LogLevel.Info);

			//try
			//{
				new OresToFieldGuideProgram(programArguments).Run();
			//}
			//catch (Exception e)
			//{
				//ConsoleLogHelper.WriteLine($"Exception has been thrown. {e}", LogLevel.Fatal);
			//}

			ConsoleLogHelper.WriteLine("Press any key to exit...", LogLevel.Info);
			Console.ReadKey();
		}

		private static bool TryGetProgramArguments(out ProgramArguments programArguments)
		{
			programArguments = new ProgramArguments();
			try
			{
				programArguments.ModpackFolder = CommonUtil.GetModpackDirectory(Directory.GetCurrentDirectory()).FullName;
				programArguments.DataFolder = GetDataDirectory();
				programArguments.WhitelistedPatchouliEntryFilenames = GetWhitelistedPatchoulyEntryFilenames();
			}
			catch (Exception e)
			{
				ConsoleLogHelper.WriteLine("An Exception has been Thrown! " + e, LogLevel.Fatal);
				return false;
			}
			return true;
		}

		private static string GetDataDirectory()
		{
			var cwd = Directory.GetCurrentDirectory();

			var projectFolder = cwd.Substring(0, cwd.IndexOf(PROJECT_NAME) + PROJECT_NAME.Length);

			string languageFilesFolder = Path.Combine(projectFolder, "data");
			if (!Directory.Exists(languageFilesFolder))
			{
				throw new DirectoryNotFoundException($"The \"data\" folder was not found in {languageFilesFolder}");
			}
			return languageFilesFolder;
		}



		private static string[] GetWhitelistedPatchoulyEntryFilenames()
		{
			return
			[
				"hazards",
				"ore_basics",
				"surface_kaolin"
			];
		}
	}
}
