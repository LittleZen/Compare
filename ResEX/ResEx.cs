using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

class ImageResolutionChecker
{
    public static List<string> LowResArrayImage = new List<string>();
    public static int count = 0;

    static void Main()
    {
        /*  Example
        int maxWidth = 1280;
        int maxHeight = 720;
        */

        int maxWidth = 865;
        int maxHeight = 903;

        Console.Write("Enter the root folder path> ");
        string rootFolder = Console.ReadLine().Replace("\"", "");

        if (!Directory.Exists(rootFolder))
        {
            Console.WriteLine("The specified root folder does not exist.");
            return;
        }

        Console.WriteLine($"\nSearching for images below {maxWidth}x{maxHeight} resolution...");

        CheckImageResolution(rootFolder, maxWidth, maxHeight);

        GC.Collect();

        DeleteItems(LowResArrayImage);

        Console.WriteLine($"\nImage resolution check completed! DELETED: {count} files");
        Console.ReadLine();
        Main();
    }

    static void CheckImageResolution(string folderPath, int maxWidth, int maxHeight)
    {
        try
        {
            string[] files = Directory.GetFiles(folderPath);

            foreach (string file in files)
            {
                Debug.WriteLine($"LOADING: {file}");
                if (IsImageFile(file))
                {
                    try
                    {
                        Bitmap img = new Bitmap(file);
                        if (img.Width <= maxWidth && img.Height <= maxHeight)
                        {
                            Console.WriteLine($"Image below resolution found: {file}");
                            LowResArrayImage.Add(file);
                            count++;
                        }
                        img.Dispose();
                    }
                    catch (System.ArgumentException)
                    {
                        Debug.WriteLine($"Unrecognize file format type detected for: {file}");
                    }
                    catch(Exception) 
                    {
                        throw new Exception($"Exception FOUND!");
                    }
                }
            }

            string[] subdirectories = Directory.GetDirectories(folderPath);

            foreach (string subdirectory in subdirectories)
            {
                CheckImageResolution(subdirectory, maxWidth, maxHeight);
            }

            // Forcing garbage collections
            files = null;
            subdirectories = null;
        }
        catch (UnauthorizedAccessException)
        {
            throw new Exception("UnauthorizedAccessException");
        }
    }

    static bool IsImageFile(string filePath)
    {
        try
        {
            string extension = Path.GetExtension(filePath);

            // Cast extentions of a file to support all format required 
            if(extension == ".heic" || extension ==  ".webp")
            {
                Path.ChangeExtension(filePath, ".png");
            }

            string[] imageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp"};
            return Array.IndexOf(imageExtensions, extension.ToLower()) != -1;
        }
        catch(Exception ex)
        {
            throw new Exception($"{ex}");
        }
    }

    static void DeleteItems(List<string> LowResArrayImage)
    {
        foreach(string file in LowResArrayImage)
        {
            Console.WriteLine($"[DELETING]:\t{file}");
            File.Delete(file);
        }
    }
}
