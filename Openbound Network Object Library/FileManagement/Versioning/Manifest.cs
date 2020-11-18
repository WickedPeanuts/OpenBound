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
        public static bool VerifyMD5Checksum(string basePath, ApplicationManifest applicationManifest)
        {
            Dictionary<string, byte[]> checksum = applicationManifest.CurrentVersionFileList.Checksum;
            List<string> fileList = applicationManifest.CurrentVersionFileList.ToBeDownloaded;

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

        /// <summary>
        /// Generates a new ApplicationManifest object after scanning the folder
        /// </summary>
        /// <param name="currentVersionFolderPath"></param>
        /// <param name="newVersionFolderPath"></param>
        public static ApplicationManifest GenerateChecksumManifest(string currentVersionFolderPath, string newVersionFolderPath)
        {
            //Create checksum of source directory
            FileList sourceDirectoryFileList = GetMissingInvalidAndOutdatedFiles(GenerateMD5Checksum(newVersionFolderPath), currentVersionFolderPath);

            //Load old manifest
            ApplicationManifest previousManifest = null;

            if (sourceDirectoryFileList.Checksum.ContainsKey(NetworkObjectParameters.ManifestFilename))
            {
                string manifestFilename = $@"{currentVersionFolderPath}\{NetworkObjectParameters.ManifestFilename}{NetworkObjectParameters.ManifestExtension}";
                string fileContent = File.ReadAllText(manifestFilename);

                previousManifest = ObjectWrapper.Deserialize<ApplicationManifest>(fileContent);
                previousManifest.PreviousManifest = null; // Erase older manifest information in order to avoid unecessary data to be written

                //Rename old manifest
                string newFileName = $@"{currentVersionFolderPath}\{NetworkObjectParameters.ManifestFilename}.{previousManifest.ID}.{NetworkObjectParameters.ManifestExtension}";
                File.Move(manifestFilename, newFileName);
            }

            //Create the current (newest) manifest
            return new ApplicationManifest(previousManifest, sourceDirectoryFileList);
        }
    }
}
