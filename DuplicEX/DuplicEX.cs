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
    internal static class DuplicEX
    {
        public const int MAX_SUB_FOLDER = 1000; // currently not used
        public static List<string> Hash = new List<string>(); // A list of all MD5 from the files
        public static List<string> FilesPath = new List<string>(); // A list of all MD5 from the files
        public static List<int> Duplicates = new List<int>(); // maybe not useful anymore  
        public static int countDuplicates = 0;
        public static int TotalFile = 0;


        static bool GetFileResources(string path)
        {
            if (path != null)
            {
                try
                {
                    string[] filePaths = Directory.GetFiles(path);
                    var dirName = new DirectoryInfo(path).Name;
                    TotalFile = filePaths.Count();
                    for (int i = 0; i < filePaths.Count(); i++)
                    {
                        FilesPath.Add(filePaths[i]);
                        Hash.Add(CalculateHash(filePaths[i]));
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
        static string CalculateHash(string filename)
        {
#if (!MD5) // it use SHA512 if MD5 is not available

// using SHA512 algorith, slower but more precise
            try
            {
                using (var sha = SHA512.Create())
                {
                    using (var stream = File.OpenRead(filename))
                    {
                        var hash = sha.ComputeHash(stream);
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                    }
                }
            }
            catch (Exception ex)
            {
                AwaitForExit($"Error during Hash calculation\n{ex}");
                return null;
            }
#else
            try
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(filename)) // read the file passed as parameter
                    {
                        var hash = md5.ComputeHash(stream); // calculate the MD5 of the file passed as parameter
                        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant(); // clean and return the hash
                    }
                }
            }
            catch (Exception ex)
            {
                AwaitForExit($"Error during Hash calculation\n{ex}");
                return null;
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
            ResetGlobal(); // RESET GLOBAL VAR 
#if MD5
            Console.Write("\n[MD5] Give me a path:");
            Console.WriteLine();
#else
             Console.WriteLine("\n[SHA512] Give me a path:");
             Console.WriteLine();
#endif
            var startTime = DateTime.Now; // catch the initial time (before execution)

            string path1 = Console.ReadLine().Replace("\"", ""); // get path from user input
            bool status = false; // Check list status 

            try // equal to "if Direcotry exist", but catch more exception  
            {
                string[] filePaths = Directory.GetFiles(path1); // return all files from the directory specified
                status = GetFileResources(path1); // load a list of all files path retreived from the directory passed as input + load the list of hashes for every files in the directory passed as input
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unable to retreive information from the path specified:\n{ex}");
                Main(); //just relaunch it if errors occur
            }

            if (!status) // returned from the function GetFileResources. If false it exited wrong
                AwaitForExit("Failed to retreive information, [DEBUG: status return false]");

            Console.WriteLine("\n> Finding duplicates...\n");
            GetDiff(); // Engine, this function verify if occur any duplicates

            Console.WriteLine("\nDuplicates:\n");

            // cycle the Duplicates list and removes all duplicates
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

            var endTime = DateTime.Now; // catch the time when execution end
            TimeSpan timeDiff = endTime - startTime; // calculate difference between time start and end 
            var converted = timeDiff.ToString().Replace("00:00:",""); //clean the output string and prepare it to the output
            Console.WriteLine($"\nStart:\t\t{startTime}\nEnd:\t\t{endTime}\nEllapsed:\t{converted}s\nFile Checked:\t{TotalFile}");
            Thread.Sleep(3000);

            Main();
        }     
        
        public static void GetDiff() // Engine
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
                    }
                } 
            }
        }
        public static void ResetGlobal()
        {
            // Reset Global Var, before restart
            countDuplicates = 0;
            TotalFile = 0;
            Hash.Clear();
            FilesPath.Clear();
            Duplicates.Clear();
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
