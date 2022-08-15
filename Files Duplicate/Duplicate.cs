using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Files_Duplicate
{
    internal static class Duplicate
    {
        public const int MAX_SUB_FOLDER = 1000;
        static void Main()
        {
            // string path1 = @"F:\1 - Linux SubSystem\1 - Leak\odrive\missy\missypwns-3"; // Subfolder, list differences from that
            Console.WriteLine("Give me a path:");
            string path1 = Console.ReadLine();
            string[] filePaths = null;
            string[] arr = null;

            try // equal to If Direcotry exist 
            {
                filePaths = Directory.GetFiles(path1);
                arr = GetFileHash(path1);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Error:\n{ex}");
                Main(); //just relaunch it 
            }

            Console.WriteLine("\nRepeated files:\n");

            string DuplicateDestination = CreateSubFolder(path1);    
            int counter = 0; // simple counter to missmatched files

            // THis part must be changed 
            string[] dist = arr.Distinct().ToArray();
            Console.WriteLine("\n");
            foreach (string s  in dist)
            {
                Console.WriteLine(s);
            }
            Console.ReadLine();
            return; // REMOVE IT 

            foreach (var number in arr.GroupBy(x => x))
            {
                //Console.WriteLine(number.Count());
                if ((number.Count() - 1) != 0)
                {
                    //Console.WriteLine(number.Key + " repeats " + (number.Count() - 1) + " times");
                    string destination = Path.Combine(DuplicateDestination, Path.GetFileName(filePaths[counter]));
                    //Console.WriteLine($"> {destination}");
                    File.Copy(filePaths[counter], destination);
                    Console.WriteLine("\t> " + filePaths[counter] + "\t\t\t[repeats " + (number.Count() - 1) + " times]");
                }
                counter++;
            }
            Main();
        }

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
                catch (Exception ex)
                {
                    AwaitForExit($"ERRORE\n{ex}");
                }
                counter++;
            }

            if (counter == MAX_SUB_FOLDER)
            {
                AwaitForExit("MAX SUB FOLDER REACHED!");
            }
            return subfolderpath;
        }

        public static string Parser(this string s)
        {
            StringBuilder sb = new StringBuilder(s);

            sb.Replace("&", "");
            sb.Replace(",", "");
            sb.Replace("  ", "");
            sb.Replace(" ", "");
            sb.Replace("'", "");
            //sb.Replace(".", "");
            sb.Replace(@"\", "");
            sb.Replace(@"{", "");
            sb.Replace(@"}", "");
            sb.Replace(@"(", "");
            sb.Replace(@")", "");
            sb.Replace(@"-", "");

            return sb.ToString().ToLower();
        }

        static string[] GetFileHash(string path)
        {
            if (path != null)
            {
                try
                {
                    string[] filePaths = Directory.GetFiles(path);
                    string[] HashPath = new string[filePaths.Count()];
                    var dirName = new DirectoryInfo(path).Name;

                    for (int i = 0; i < filePaths.Count(); i++)
                    {
                        HashPath[i] = CalculateMD5(filePaths[i]);
                        Console.WriteLine($"\t- Loading file: {i}/{filePaths.Count()} - Directory: [{dirName}] - Hash: {HashPath[i]}");
                    }
                    if (HashPath != null)
                    {
                        Console.WriteLine($"\n> List of hash correctly loaded!\n");
                        return HashPath;
                    }
                    else
                        AwaitForExit("HashPath is null, unable to retreive file list!");
                }
                catch (Exception ex)
                {
                    AwaitForExit($"ERROR:\n{ex}");
                }
            }
            AwaitForExit("Path not exist!");
            return null;
        }

        // Calculate MD5 of a file passed as string (path) (1 file)
        static string CalculateMD5(string filename)
        {
            /*
            using (var sha = SHA512.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = sha.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            */
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
            
        }
        private static void AwaitForExit(string text)
        {
            Console.WriteLine(text); // write the text passed as parameter
            Console.ReadKey();  // await user read the text
            Environment.Exit(1); // Exit with error code 1 
        }
    }
}
