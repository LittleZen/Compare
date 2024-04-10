using System;
using System.Diagnostics;
using System.IO;

class BlankFolder
{
    static void Main()
    {
        Console.Write("\n>Give me a path:");
        Console.WriteLine();
        string rootFolder = Console.ReadLine().Replace("\"", ""); 

        if (!Directory.Exists(rootFolder))
        {
            Console.WriteLine("The specified root folder does not exist.");
            return;
        }

        Console.WriteLine($"\n> Searching for empty folders in: [{rootFolder}]\n\n");

        CheckEmptyFolders(rootFolder);

        Console.WriteLine("\nEmpty folder check completed!");

        Console.ReadLine();
        Console.Clear();
        Main();
    }

    static void CheckEmptyFolders(string folderPath)
    {
        try
        {
            string[] subdirectories = Directory.GetDirectories(folderPath);

            foreach (string subdirectory in subdirectories)
            {
                CheckEmptyFolders(subdirectory);
            }
#if DEBUG
            Debug.WriteLine($"Looking for: {folderPath}");
#endif

            if (Directory.GetFileSystemEntries(folderPath).Length == 0)
            {
                Console.WriteLine($"Empty folder found:\t{folderPath}");
                Directory.Delete(folderPath); // [DON'T TOUCH] not recusively delete, since you can not be sure sub-folders are empty. Let's wait and manually lookup
            }
        }
        catch (UnauthorizedAccessException)
        {
            throw new Exception("UnauthorizedAccessException");
        }
    }
}
