using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace OpenBound_Network_Object_Library.FileManagement
{
    public class Checksum
    {
        public class FileList
        {
            public List<string> ToBeDeleted;
            public List<string> ToBeDownloaded;
            public List<string> ToBeIgnored;
            public Dictionary<string, byte[]> Checksum;

            public FileList()
            {
                ToBeDeleted = new List<string>();
                ToBeDownloaded = new List<string>();
                ToBeIgnored = new List<string>();
                Checksum = new Dictionary<string, byte[]>();
            }
        }

        public static Dictionary<string, byte[]> GenerateMD5Checksum(string baseDirectory)
        {
            Dictionary<string, byte[]> checksumDictionary = new Dictionary<string, byte[]>();

            foreach (string path in IOManager.ListAllFiles(baseDirectory))
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = new StreamReader(path))
                    {
                        checksumDictionary.Add(
                            path.Replace($"{baseDirectory}\\", ""),
                            md5.ComputeHash(stream.BaseStream));
                    }
                }
            }

            return checksumDictionary;
        }

        /// <summary>
        /// Compares the current game files with the ones provided on the parameter and 
        /// returns a list of list of all the files that should be downloaded/replaced by the server's version
        /// </summary>
        /// <param name="expectedFileChecksumDictionary"></param>
        /// <returns></returns>
        public static FileList GetMissingInvalidAndOutdatedFiles(Dictionary<string, byte[]> expectedFileChecksumDictionary)
        {
            Dictionary<string, byte[]> currentFilesChecksumDictionary = GenerateMD5Checksum(Directory.GetCurrentDirectory());
            FileList fileList = new FileList();

            foreach(KeyValuePair<string, byte[]> fileChecksum in expectedFileChecksumDictionary)
            {
                if (currentFilesChecksumDictionary.ContainsKey(fileChecksum.Key))
                {
                    if (currentFilesChecksumDictionary[fileChecksum.Key].SequenceEqual(fileChecksum.Value))
                    {
                        fileList.ToBeIgnored.Add(fileChecksum.Key);
                        continue;
                    }
                }

                fileList.ToBeDownloaded.Add(fileChecksum.Key);
                fileList.Checksum.Add(fileChecksum.Key, fileChecksum.Value);
            }

            foreach(KeyValuePair<string, byte[]> unusedFiles in currentFilesChecksumDictionary)
            {
                if (!expectedFileChecksumDictionary.ContainsKey(unusedFiles.Key))
                {
                    fileList.ToBeDeleted.Add(unusedFiles.Key);
                }
            }

            return fileList;
        }

        public static void GenerateChecksumManifest()
        {

        }
    }
}
