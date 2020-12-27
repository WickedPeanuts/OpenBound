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
        private static string BuildPatchPath(string outputPath, PatchHistory patchHistory) =>
            @$"{outputPath}\{patchHistory.BuildPatchPath}";

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
        public static List<PatchHistory> UnpackPatchList(string gameFolder, List<PatchHistory> patchEntryList, Action<PatchHistory> onStartUnpacking = null, Action<PatchHistory, bool, Exception> onUnpack = null)
        {
            List<PatchHistory> failedPatchList = new List<PatchHistory>();

            string patchUnpackDestinationPath = BuildUnpackDestinationPatchPath(gameFolder);
            string patchListBaseFolder = BuildUnpackTemporaryFolder(gameFolder);

            //Creates the output directory
            Directory.CreateDirectory(patchUnpackDestinationPath);

            foreach (PatchHistory pH in patchEntryList)
            {
                onStartUnpacking?.Invoke(pH);

                try
                {
                    if (UnpackPatch(patchUnpackDestinationPath, $@"{patchListBaseFolder}\{pH.BuildPatchPath}"))
                    {
                        onUnpack?.Invoke(pH, true, null);
                    }
                    else
                    {
                        failedPatchList.Add(pH);
                        onUnpack?.Invoke(pH, false, null);
                    }
                }
                catch(Exception ex)
                {
                    failedPatchList.Add(pH);
                    onUnpack?.Invoke(pH, false, ex);
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
            //Saving the previous patchHistory
            string patchHistoryPath = $@"{destinationFolder}\{NetworkObjectParameters.LatestPatchHistoryFilename}";

            PatchHistory patchHistory;

            try
            {
                patchHistory = ObjectWrapper.DeserializeFile<PatchHistory>(patchHistoryPath);
                File.Copy(patchHistoryPath, $@"{destinationFolder}\{patchHistory.PatchHistoryPath}");
            }
            catch (Exception) { }

            //Extracting zip files into the directory
            using (ZipArchive patch = ZipFile.OpenRead(patchPath))
                patch.ExtractToDirectory(destinationFolder, overwriteFiles: true);

            //Reading Manifest
            patchHistory = ObjectWrapper.DeserializeFile<PatchHistory>(patchHistoryPath);

            //Verify game cache integrity
            return Manifest.VerifyMD5Checksum(destinationFolder, patchHistory);
        }

        public static PatchHistory GenerateUpdatePatch(string currentVersionFolderPath, string newVersionFolderPath, string outputPackagePath, string patchHistoryFilePath, string newPatchVersionName)
        {
            //Create ApplicationManifest given the new and the old game folder
            FileList fileList =  Manifest.GenerateFileList(currentVersionFolderPath, newVersionFolderPath, newPatchVersionName);

            PatchHistory newPatchHistory = new PatchHistory(
                ObjectWrapper.DeserializeFile<PatchHistory>(patchHistoryFilePath),
                fileList, newPatchVersionName);

            using (ZipArchive zipArchive = ZipFile.Open(BuildPatchPath(outputPackagePath, newPatchHistory), ZipArchiveMode.Update))
            {
                foreach (string filePath in newPatchHistory.FileList.ToBeDownloaded)
                {
                    zipArchive.CreateEntryFromFile($@"{newVersionFolderPath}\{filePath}", filePath, CompressionLevel.Optimal);
                }

                //Save the patch history
                File.WriteAllText(patchHistoryFilePath, ObjectWrapper.Serialize(newPatchHistory, Formatting.Indented));

                //Save the PatchHistory file into the temporary folder, add it into the zip and delete it
                zipArchive.CreateEntryFromFile(patchHistoryFilePath, NetworkObjectParameters.LatestPatchHistoryFilename);
                
                return newPatchHistory;
            }
        }
    }
}
