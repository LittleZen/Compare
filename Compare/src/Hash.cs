using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace Compare.src
{
    class Hash
    {
        // Calculate the md5 of all file inside an array (an array of file)
        public static string[] GetFileHash(string path)
        {
            string[] filePaths = Directory.GetFiles(path);

            string[] HashPath = new string[filePaths.Count()];

            for (int i = 0; i < filePaths.Count(); i++)
            {
                HashPath[i] = CalculateMD5(filePaths[i]);
            }
            return (HashPath);
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
        public static string CalculateMD5(string filename)
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
