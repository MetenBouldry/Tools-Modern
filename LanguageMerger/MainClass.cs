//Uncomment this to write json in a Prettified format, only really useful for debugging the output
#define PRETTY_PRINT
//Uncomment this to make the program overwrite all the files within .minecraf/kubejs/assets/../tfg_ores/
#define OVERWRITE_FILES

using System.Security.Cryptography.X509Certificates;
using Common;

namespace LanguageMerger;
public class MainClass
{
    #region Constants
    public const string PROJECT_NAME = "LanguageMerger";
    public const string LANGUAGE_FILES = "LanguageFiles";
    public const string MINECRAFT = "minecraft";
    public const string TOOLS = "tools";
    #endregion

    public static void Main(string[] args)
    {
        Console.WriteLine("Generating Localization Files!");

        if(!TryGetProgramArguments(out ProgramArguments programArguments))
        {
            ConsoleLogHelper.WriteLine("Failed to get Program's Arguments, Press any key to exit...", LogLevel.Error);
            Console.ReadKey();
            return;
        }

        ConsoleLogHelper.WriteLine("Arguments have been obtained! Printing...", LogLevel.Info);
        ConsoleLogHelper.WriteLine(programArguments.ToString(), LogLevel.Message);

        var programInstance = new LanguageMergerProgram(programArguments);
        bool result = false;
        try
        {
            var task = programInstance.Run();
            task.Wait();

            result = task.Result;
        }
        catch (Exception e)
        {
            ConsoleLogHelper.WriteLine($"Exception has been thrown. {e}", LogLevel.Fatal);
            result = false;
        }

        if(result)
        {
            ConsoleLogHelper.WriteLine("Success :D! Press any key to exit...", LogLevel.Info);
        }
        else
        {
            ConsoleLogHelper.WriteLine("Failure D:! Press any key to exit...", LogLevel.Error);
        }
        Console.ReadKey();
    }


    private static bool TryGetProgramArguments(out ProgramArguments programArguments)
    {
        programArguments = new ProgramArguments();
        try
        {
            programArguments.modpackDirectory = CommonUtil.GetModpackDirectory(Directory.GetCurrentDirectory());
            programArguments.kjsAssetsFolder = CommonUtil.GetKJSAssetsFolder(programArguments.modpackDirectory);
            programArguments.languageFilesFolder = GetLanguageFilesFolder();
            programArguments.shouldOverwriteFiles = GetShouldOverwriteFiles();
            programArguments.shouldPrettyPrint = GetShouldPrettyPrint();
        }
        catch(Exception e)
        {
            ConsoleLogHelper.WriteLine($"Exception has been thrown. {e}", LogLevel.Fatal);
            return false;
        }
        return true;
    }

    private static DirectoryInfo GetLanguageFilesFolder()
    {
        var cwd = Directory.GetCurrentDirectory();

        var projectFolder = cwd.Substring(0, cwd.IndexOf(PROJECT_NAME) + PROJECT_NAME.Length);

		string languageFilesFolder = Path.Combine(projectFolder, LANGUAGE_FILES);
        if(!Directory.Exists(languageFilesFolder))
        {
            throw new DirectoryNotFoundException($"The \"{LANGUAGE_FILES}\" folder was not found in {languageFilesFolder}");
        }
        return new DirectoryInfo(languageFilesFolder);
    }

    private static bool GetShouldOverwriteFiles()
    {
#if PRETTY_PRINT
        return true;
#else
        return false;
#endif
    }

    private static bool GetShouldPrettyPrint()
    {
#if OVERWRITE_FILES
        return true;
#else
        return false;
#endif
    }
}