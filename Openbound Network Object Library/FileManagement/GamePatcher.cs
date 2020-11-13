using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;
using OpenBound_Network_Object_Library.FileManagement;

namespace OpenBound_Network_Object_Library.FileManagement
{
    public class GamePatcher
    {
        public static bool ApplyUpdatePatch(string gameFolderPath, string patchPath)
        {
            try
            {
                //Extracting zip files into the directory
                using (ZipArchive patch = ZipFile.OpenRead(patchPath))
                    patch.ExtractToDirectory(gameFolderPath, overwriteFiles: true);

                //Reading Manifest
                string manifestOldPath = $@"{gameFolderPath}/{NetworkObjectParameters.ManifestFilename}{NetworkObjectParameters.ManifestExtension}";
                ApplicationManifest appManifest = ObjectWrapper.Deserialize<ApplicationManifest>(File.ReadAllText(manifestOldPath));

                //Moving Manifest
                string manifestNewPath = $@"{gameFolderPath}/{NetworkObjectParameters.ManifestFilename}-{appManifest.ID}-{NetworkObjectParameters.ManifestExtension}";
                File.Move(manifestOldPath, manifestNewPath);

                //Files to be deleted
                foreach (string toBeDeletedFile in appManifest.CurrentVersionFileList.ToBeDeleted)
                    File.Delete(toBeDeletedFile);

                //Verify game cache integrity
                return Manifest.VerifyMD5Checksum(gameFolderPath, appManifest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            return false;
        }

        public static void GenerateUpdatePatch(string currentVersionFolderPath, string newVersionFolderPath, string outputPackagePath)
        {
            try
            {
                //Create ApplicationManifest given the new and the old game folder
                ApplicationManifest appManifest = Manifest.GenerateChecksumManifest(currentVersionFolderPath, newVersionFolderPath);

                string tmpAppManifestFilename = Path.GetTempFileName();

                using (ZipArchive zipArchive
                    = ZipFile.Open(
                        $@"{outputPackagePath}/{NetworkObjectParameters.GamePatchFilename}{NetworkObjectParameters.GamePatchExtension}",
                    ZipArchiveMode.Update))
                {
                    foreach (string filePath in appManifest.CurrentVersionFileList.ToBeDownloaded)
                    {
                        zipArchive.CreateEntryFromFile($@"{newVersionFolderPath}/{filePath}", filePath, CompressionLevel.Optimal);
                    }

                    //Save the manifest file into the temporary folder, add it into the zip and delete it
                    File.WriteAllText(tmpAppManifestFilename, ObjectWrapper.Serialize(appManifest, Formatting.Indented));
                    zipArchive.CreateEntryFromFile(tmpAppManifestFilename, NetworkObjectParameters.ManifestFilename + NetworkObjectParameters.ManifestExtension);
                    File.Delete(tmpAppManifestFilename);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
