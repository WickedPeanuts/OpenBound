using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace OpenBound_Network_Object_Library.FileManagement.Versioning
{
    public class Manifest
    {
        public static bool VerifyMD5Checksum(string basePath, PatchHistory patchHistory)
        {
            Dictionary<string, byte[]> checksum = patchHistory.FileList.Checksum;
            List<string> fileList = patchHistory.FileList.ToBeDownloaded;

            bool result = true;

            foreach (string file in fileList)
            {
                using (var md5 = MD5.Create())
                {
                    using (var stream = new StreamReader($@"{basePath}\{file}"))
                    {
                        if (!(result &= checksum[file].SequenceEqual(md5.ComputeHash(stream.BaseStream))))
                            return false;
                    }
                }
            }

            return true;
        }

        private static Dictionary<string, byte[]> GenerateMD5Checksum(string baseDirectory)
        {
            Dictionary<string, byte[]> checksumDictionary = new Dictionary<string, byte[]>();

            foreach (string filename in IOManager.ListAllFiles(baseDirectory))
            {
                //Ignore manifest
                if (filename.Contains(NetworkObjectParameters.ManifestFilename) && filename.Contains(NetworkObjectParameters.ManifestExtension))
                    continue;

                using (var md5 = MD5.Create())
                {
                    using (var stream = new StreamReader(filename))
                    {
                        checksumDictionary.Add(
                            filename.Replace($"{baseDirectory}\\", ""),
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
        public static FileList GetMissingInvalidAndOutdatedFiles(Dictionary<string, byte[]> expectedFileChecksumDictionary, string currentComparingDirectory)
        {
            Dictionary<string, byte[]> currentFilesChecksumDictionary = GenerateMD5Checksum(currentComparingDirectory);

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
            }

            foreach(KeyValuePair<string, byte[]> unusedFiles in currentFilesChecksumDictionary)
            {
                if (!expectedFileChecksumDictionary.ContainsKey(unusedFiles.Key))
                {
                    fileList.ToBeDeleted.Add(unusedFiles.Key);
                }
            }

            fileList.Checksum = currentFilesChecksumDictionary;

            return fileList;
        }

        public static FileList GenerateFileList(string currentVersionFolderPath, string newVersionFolderPath, string newPatchVersionName)
        {
            //Create checksum of source directory
            Dictionary<string, byte[]> newFolderChecksum = GenerateMD5Checksum(newVersionFolderPath);
            return GetMissingInvalidAndOutdatedFiles(newFolderChecksum, currentVersionFolderPath);
        }
    }
}
