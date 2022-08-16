#define MD5

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TEST
{    
    internal static class Program
    {
        public const int MAX_SUB_FOLDER = 1000;
        public static List<string> Hash = new List<string>(); // A list of all MD5 from the files
        public static List<string> FilesPath = new List<string>(); // A list of all MD5 from the files
        public static List<int> Duplicates = new List<int>(); // maybe not useful anymore  
        public static int countDuplicates = 0;

        static bool GetFileResources(string path)
        {
            if (path != null)
            {
                try
                {
                    string[] filePaths = Directory.GetFiles(path);
                    //string[] HashPath = new string[filePaths.Count()];
                    var dirName = new DirectoryInfo(path).Name;

                    for (int i = 0; i < filePaths.Count(); i++)
                    {
                        FilesPath.Add(filePaths[i]);
                        Hash.Add(CalculateMD5(filePaths[i]));
                        Console.WriteLine($"\t- Loading file: {i + 1}/{filePaths.Count()} - Directory: [{dirName}] - Hash: {Hash[i]}");
                    }
                }
                catch (Exception ex)
                {
                    AwaitForExit($"ERROR:\n{ex}");
                    return false;
                }
            }
            else
            {
                AwaitForExit("Path is null!");
                return false;
            }
            return true;
        }

        static string CalculateMD5(string filename)
        {
#if (DEBUG && !MD5) 

// using SHA512 algorith, slower but more precise
            using (var sha = SHA512.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = sha.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }

#else
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
#endif
        }
        private static void AwaitForExit(string text)
        {
            Console.WriteLine(text); // write the text passed as parameter
            Console.ReadKey();  // await user read the text
            Environment.Exit(1); // Exit with error code 1 
        }
        static void Main()
        {
            // RESET GLOBAL VAR
            countDuplicates = 0;
            Hash.Clear();
            FilesPath.Clear();
            Duplicates.Clear();

#if MD5
            Console.Write("\n[MD5] Give me a path:");
            Console.WriteLine();
#else
             Console.WriteLine("\n[SHA512] Give me a path:");
             Console.WriteLine();
#endif

            string path1 = Console.ReadLine().Replace("\"", ""); // need to be protect
            bool status = false; // Check list status 

            try // equal to If Direcotry exist 
            {
                string[] filePaths = Directory.GetFiles(path1);
                status = GetFileResources(path1);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error:\n{ex}");
                Main(); //just relaunch it 
            }

            if (!status)
                Console.WriteLine("Better exit now");

            Console.WriteLine("\nStarting Checks\n");
            GetDiff();


            string[] dist = Hash.Distinct().ToArray();


            Console.WriteLine("\nDistinct Elements\n");

            foreach (int number in Duplicates)
            {
                try
                {
                    File.Delete(FilesPath[number]);
                    Console.WriteLine($"> DELETING: {FilesPath[number]}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unable to delete due:\n{ex}");
                    Console.ReadKey(); // MAY BE DELETED IN FUTURE
                }
               
            }

            Console.WriteLine($"\nTotal Duplicates: {countDuplicates}");

            Main();
        }

        public static void GetDiff()
        {
            for(int i = 0; i < Hash.Count(); i++)
            {
                if (Hash[i] != "0")
                {
                    for (int j = 0; j < Hash.Count(); j++)
                    {
                        if ((Hash[j] != null) && (Hash[i] == Hash[j]) && (i != j) && (Hash[j] != "0"))
                        {
                            Duplicates.Add(j);
                            Console.WriteLine($"\n>===========\n{Path.GetFileName(FilesPath[i])}\nHash: {Hash[i]}\n\n{Path.GetFileName(FilesPath[j])}\nHash: {Hash[j]}\n===========");
                            Hash[j] = "0"; // so the hash will not be researched once reached. Is a stupid solution, but atm necessary
                            countDuplicates++;
                        }
                        //Console.Write("\n");    
                    }
                } 
            }
        }

        // === THE FOLLOWING FUNCTIONS ARE NOT CURRENTLY USED, BUT THEY WILL BE ADDED SOON AS NEW FEATURES (INSTEAD REMOVE, MOVE DUPLICATES FILE IN A NEW FOLDER) === //
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
    }
}
