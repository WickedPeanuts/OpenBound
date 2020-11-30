using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using OpenBound_Network_Object_Library.FileManagement;
using System.Linq;
using OpenBound_Network_Object_Library.FileManagement.Versioning;

namespace OpenBound_Network_Object_Library.FileManagement
{
    public class GamePatcher
    {
        private static string BuildPatchPath(string outputPath, ApplicationManifest applicationManifest) =>
            @$"{outputPath}\{applicationManifest.BuildPatchPath}";

        private static string BuildUnpackTemporaryFolder(string baseDirectory) =>
            $@"{baseDirectory}\{NetworkObjectParameters.PatchTemporaryPath}";

        private static string BuildUnpackDestinationPatchPath(string baseDirectory) =>
            $@"{BuildUnpackTemporaryFolder(baseDirectory)}\{NetworkObjectParameters.PatchUnpackPath}";

        /// <summary>
        /// Unpack a list of patchs, given the folder the patchs are in and a list of <see cref="PatchEntry"/>.
        /// </summary>
        /// <param name="gameFolder"></param>
        /// <param name="patchEntryList"></param>
        /// <param name="onStartUnpacking"></param>
        /// <param name="onUnpack"></param>
        /// <returns></returns>
        public static List<PatchEntry> UnpackPatchList(string gameFolder, List<PatchEntry> patchEntryList, Action<PatchEntry> onStartUnpacking = null, Action<PatchEntry, bool, Exception> onUnpack = null)
        {
            List<PatchEntry> failedPatchList = new List<PatchEntry>();

            string patchUnpackDestinationPath = BuildUnpackDestinationPatchPath(gameFolder);
            string patchListBaseFolder = BuildUnpackTemporaryFolder(gameFolder);

            //Creates the output directory
            Directory.CreateDirectory(patchUnpackDestinationPath);

            foreach (PatchEntry pE in patchEntryList)
            {
                onStartUnpacking?.Invoke(pE);

                try
                {
                    if (UnpackPatch(patchUnpackDestinationPath, $@"{patchListBaseFolder}\{pE.Path}"))
                    {
                        onUnpack?.Invoke(pE, true, null);
                    }
                    else
                    {
                        failedPatchList.Add(pE);
                        onUnpack?.Invoke(pE, false, null);
                    }
                }
                catch(Exception ex)
                {
                    failedPatchList.Add(pE);
                    onUnpack?.Invoke(pE, false, ex);
                }
            }

            return failedPatchList;
        }

        /// <summary>
        /// Extracts the files on the patch and returns true verified files are correct.
        /// </summary>
        /// <param name="destinationFolder"></param>
        /// <param name="patchPath"></param>
        /// <returns></returns>
        private static bool UnpackPatch(string destinationFolder, string patchPath)
        {
            //Extracting zip files into the directory
            using (ZipArchive patch = ZipFile.OpenRead(patchPath))
                patch.ExtractToDirectory(destinationFolder, overwriteFiles: true);

            //Reading Manifest
            string patchHistoryPath = $@"{destinationFolder}\{NetworkObjectParameters.PatchHistoryFilename}";
            PatchHistory patchHistory = ObjectWrapper.DeserializeFile<PatchHistory>(patchHistoryPath);

            //Moving Manifest
            //string manifestNewPath = $@"{destinationFolder}\{NetworkObjectParameters.ManifestFilename}-{appManifest.ID}{NetworkObjectParameters.ManifestExtension}";
            //File.Move(patchHistoryPath, manifestNewPath);

            //Files to be deleted
            //foreach (string toBeDeletedFile in appManifest.CurrentVersionFileList.ToBeDeleted)
                //File.Delete(toBeDeletedFile);

            //Verify game cache integrity
            return Manifest.VerifyMD5Checksum(destinationFolder, patchHistory.ApplicationManifest);
        }

        public static PatchHistory GenerateUpdatePatch(string currentVersionFolderPath, string newVersionFolderPath, string outputPackagePath, string patchHistoryFilePath, string newPatchVersionName)
        {
            //Create ApplicationManifest given the new and the old game folder
            ApplicationManifest appManifest = Manifest.GenerateChecksumManifest(currentVersionFolderPath, newVersionFolderPath, newPatchVersionName);

            using (ZipArchive zipArchive = ZipFile.Open(BuildPatchPath(outputPackagePath, appManifest), ZipArchiveMode.Update))
            {
                foreach (string filePath in appManifest.CurrentVersionFileList.ToBeDownloaded)
                {
                    zipArchive.CreateEntryFromFile($@"{newVersionFolderPath}\{filePath}", filePath, CompressionLevel.Optimal);
                }

                //Save the patch history 
                PatchHistory patchHistory = PatchHistory.CreatePatchHistoryInstance(patchHistoryFilePath);
                patchHistory.AddPatchEntry(appManifest);
                File.WriteAllText(patchHistoryFilePath, ObjectWrapper.Serialize(patchHistory, Formatting.Indented));

                //Save the PatchHistory file into the temporary folder, add it into the zip and delete it
                zipArchive.CreateEntryFromFile(patchHistoryFilePath, NetworkObjectParameters.PatchHistoryFilename);
                
                return patchHistory;
            }
        }

        /// <summary>
        /// Merge two patchs into one zip file
        /// </summary>
        /// <param name="patch1"></param>
        /// <param name="patch2"></param>
        /// <param name="outputPackagePath"></param>
        /// <returns></returns>
        /*
        public static ApplicationManifest MergeUpdatePatch(string patch1, string patch2, string outputPackagePath, string patchHistoryFilePath)
        {
            using (ZipArchive zipArchive1 = ZipFile.Open(patch1, ZipArchiveMode.Read))
            using (ZipArchive zipArchive2 = ZipFile.Open(patch2, ZipArchiveMode.Read))
            {
                string tmpFolder = @$"{Path.GetTempPath()}{Guid.NewGuid()}";
                string extractedFilePatchHistoryPath = $@"{tmpFolder}\{NetworkObjectParameters.ManifestFilename}{NetworkObjectParameters.ManifestExtension}";

                zipArchive1.ExtractToDirectory(tmpFolder, true);
                ApplicationManifest appManifest1 = ObjectWrapper.Deserialize<PatchHistory>(File.ReadAllText(extractedFilePatchHistoryPath)).ApplicationManifest;

                zipArchive2.ExtractToDirectory(tmpFolder, true);
                ApplicationManifest appManifest2 = ObjectWrapper.Deserialize<PatchHistory>(File.ReadAllText(extractedFilePatchHistoryPath)).ApplicationManifest;

                ApplicationManifest newManifest = new ApplicationManifest(appManifest1, appManifest2);
                File.WriteAllText(extractedFilePatchHistoryPath, ObjectWrapper.Serialize(newManifest, Formatting.Indented));

                using (ZipArchive outputZipArchive = ZipFile.Open(BuildPatchPath(outputPackagePath, newManifest), ZipArchiveMode.Update))
                {
                    foreach (string filePath in newManifest.CurrentVersionFileList.ToBeDownloaded)
                        outputZipArchive.CreateEntryFromFile($@"{tmpFolder}\{filePath}", filePath, CompressionLevel.Optimal);

                    outputZipArchive.CreateEntryFromFile(extractedFilePatchHistoryPath, $@"{NetworkObjectParameters.ManifestFilename}{NetworkObjectParameters.ManifestExtension}", CompressionLevel.Optimal);
                }

                Directory.Delete(tmpFolder, true);

                //Save the patch history 
                //patchHistoryFilePath
                PatchHistory patchHistory = PatchHistory.CreatePatchHistoryInstance(patchHistoryFilePath);
                patchHistory.MergePatchEntry(appManifest1, appManifest2, newManifest);
                //patchHistoryFilePath
                File.WriteAllText(patchHistoryFilePath, ObjectWrapper.Serialize(patchHistory, Formatting.Indented));

                return newManifest;
            }
        }*/
    }
}
