using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace Files_Compare
{
    internal class Program
    {
        public const int MAX_SUB_FOLDER = 1000; // define the max loop (and the max subfolder possible) for the subfolder function 

        // TODO: implement a function wich remove duplicate items (MD5 checks)
        // TODO: create a config file for the 2 path
        // Returns an array containing a list of all the md5 from the files in the folder pass as parameter -  WARNING: (should be not recursive)
        static void Main(string[] args)
        {
            string path1 = @"C:\Users\Jacopo\Desktop\x"; // Main folder
            string path2 = @"C:\Users\Jacopo\Desktop\y"; // Folder to integrate
            string[] HashPath1 = GetFileHash(path1);
            string[] HashPath2 = GetFileHash(path2);
            string[] filePaths = null;

            if(path1 == null || path2 == null || HashPath1 == null || HashPath2 == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                AwaitForExit($"\nCore information missing! ");
            }

            try // if directory exist ... same shit 
            {
                filePaths = Directory.GetFiles(path2);
                if(filePaths == null)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    AwaitForExit("ERROR: invalid filePath, unable to retreive data from source2 (path2 may be missing?)");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"ERRORE\n{ex}");
            }
            

            var ListOfDifference = GetDifference(HashPath1, HashPath2); // match differences between Hash

            if (ListOfDifference.Count != 0) // There are different files in folder 2, let's catch them
            {
                var PathListAllMissMatch = new List<string>();
                Console.WriteLine("> List of file Miss-matched:\n");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("================\n");

                int i = 1; // DEBUG MODE WILL BE OVERWRITE
                foreach (int position in ListOfDifference)
                {
                    PathListAllMissMatch.Add(filePaths[position]);
                    Console.WriteLine($"\t> ({i}) - " + filePaths[position]);
                    i++;
                }

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"\nNumber of file: {ListOfDifference.Count}");
                Console.WriteLine("================");
                Console.ForegroundColor = ConsoleColor.Gray;
                
                // Merging file
                Console.Write("\n> How would you proceed?\n\n\t- [type: <y/Y>]: Merge all the file\n\t- [type: <s/S>]: Create a subfolder with all the files missmatched\n\t- [type: <n/N>]: Close the program\nSELECT> ");
                string getinput = Console.ReadLine();
                Console.WriteLine("");

                if (getinput.Trim() == "y" || getinput.Trim() == "Y")
                {
                    // Move files
                    MoveFiles(path1, PathListAllMissMatch);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\n> Done, file/s moved!");
                }
                else if(getinput.Trim() == "s" || getinput.Trim() == "S")
                {
                    // Move files in subfolder
                    string newSubPath = CreateSubFolder(path1);
                    if(newSubPath == null)
                    {
                        AwaitForExit("newSubPath is null!");
                    }
                    MoveFiles(newSubPath, PathListAllMissMatch);
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.WriteLine("\n> Done, file/s moved!");
                }
                else if (getinput.Trim() == "n" || getinput.Trim() == "N")
                {
                    AwaitForExit("> Await user input to exit...");
                }
            }
            else // Made some extra checks, just in case  
            {
                //Checking the folder size (insert into the else because is called only if needed)
                if (DirSize(new DirectoryInfo(path1)) != DirSize(new DirectoryInfo(path2)))
                {
                    double FirstFolder = (double)DirSize(new DirectoryInfo(path1)) / 1024;
                    FirstFolder = System.Math.Round(FirstFolder, 2);
                    double SecondFolder = (double)DirSize(new DirectoryInfo(path2)) / 1024;
                    SecondFolder = System.Math.Round(SecondFolder, 2);

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"> Folders miss match!\nBUT first folder contains all files of second one, there may be duplicated files with same MD5! (Cloned files)\n\n----------\nFirst folder size: {FirstFolder} MB\nHashed File: {HashPath1.Count()}\n----------\n\n----------\nSecond folder size: {SecondFolder} MB\nHashed file: {HashPath2.Count()}\n----------");
                }
                // "else" folder have same items and size
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nFolder contains same items!");
            }

            Console.ReadKey();
        }

        private static void AwaitForExit(string text)
        {
            Console.WriteLine(text); // write the text passed as parameter
            Console.ReadKey();  // await user read the text
            Environment.Exit(1); // Exit with error code 1 
        }

        // Create a subfolder in the path specify
        public static string CreateSubFolder(string path)
        {
            int counter = 0;
            string subfolderpath = null;
            while (counter < MAX_SUB_FOLDER)
            {
                subfolderpath = Path.Combine(path, counter.ToString());
                Console.WriteLine($"Creating subfolder in: {subfolderpath} ...");
                Thread.Sleep(500);
                try
                {
                    if (!Directory.Exists(subfolderpath))
                    {
                        Directory.CreateDirectory(subfolderpath);
                        break;
                    }
                }
                catch(Exception ex)
                {
                    AwaitForExit($"ERRORE\n{ex}");
                }
                counter++;
            }
            if(counter == MAX_SUB_FOLDER)
            {
                AwaitForExit("MAX SUB FOLDER REACHED!");
            }
            return subfolderpath;
        }

        // Move all file from path2 into path1
        public static void MoveFiles(string destinationPath, List<string> FilesToMove)
        {
            foreach (string file in FilesToMove)
            {
                string destinazione = destinationPath + "\\" + Path.GetFileName(file);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"\t Working on: {Path.GetFileName(file)}");
                File.Copy(file, destinazione);
            }

            Console.ForegroundColor = ConsoleColor.Gray;
        }

        // Return the size of a folder (recursive)
        public static long DirSize(DirectoryInfo d)
        {
            long size = 0;

            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                size += fi.Length;
            }

            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                size += DirSize(di);
            }

            return size;
        }
        // Calculate the md5 of all file inside an array (an array of file)
        static string[] GetFileHash(string path)
        {
            if(path != null)
            {
                try
                {
                    string[] filePaths = Directory.GetFiles(path); // exception
                    string[] HashPath = new string[filePaths.Count()];

                    for (int i = 0; i < filePaths.Count(); i++)
                    {
                        HashPath[i] = CalculateMD5(filePaths[i]);
                    }

                    return (HashPath);
                }
                catch (Exception ex)
                {
                    AwaitForExit($"ERROR:\n{ex}");
                }
            }
            return null;
        }

        // difference between 2 arrays, return a list of all file missmatched (Difference between HashPath1 - HashPath2)
        // --> Atm in debug mode returns the number of items in the list, not the list itself
        public static List<int> GetDifference(string[] HashPath1, string[] HashPath2)
        {
            var Difference = new List<int>();
            for (int j = 0; j < HashPath2.Count(); j++)
            {
                if (!HashPath1.Contains(HashPath2[j]))
                {
                    Difference.Add(j);
                }
            }
            return Difference;
        }

        // Calculate MD5 of a file passed as string (path) (1 file)
        static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }
}
