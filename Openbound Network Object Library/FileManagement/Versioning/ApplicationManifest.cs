using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBound_Network_Object_Library.FileManagement.Versioning
{
    public class ApplicationManifest
    {
        public Guid ID;
        public FileList CurrentVersionFileList;
        public DateTime CreationDate;
        public ApplicationManifest PreviousManifest;
        public List<Guid> LocalHistory;
        
        public string BuildPatchPath => $@"{NetworkObjectParameters.GamePatchFilename}-{DateTime.UtcNow:dd-MM-yyyy}-{ID}{NetworkObjectParameters.GamePatchExtension}";
        
        public ApplicationManifest() { }

        public ApplicationManifest(ApplicationManifest applicationManifest1, ApplicationManifest applicationManifest2)
        {
            ID = Guid.NewGuid();

            CreationDate = DateTime.UtcNow;

            PreviousManifest = applicationManifest1.PreviousManifest;

            LocalHistory = (applicationManifest2.LocalHistory == null) ? new List<Guid>() : applicationManifest2.LocalHistory.ToList();
            LocalHistory.Add(ID);

            CurrentVersionFileList = new FileList();
            CurrentVersionFileList.ToBeDeleted =
                applicationManifest1.CurrentVersionFileList.ToBeDeleted.Union(
                    applicationManifest2.CurrentVersionFileList.ToBeDeleted).Except(
                    applicationManifest2.CurrentVersionFileList.ToBeDownloaded).ToList();

            CurrentVersionFileList.ToBeIgnored =
                applicationManifest1.CurrentVersionFileList.ToBeIgnored.Intersect(
                    applicationManifest2.CurrentVersionFileList.ToBeIgnored).ToList();

            foreach (KeyValuePair<string, byte[]> kvp in applicationManifest1.CurrentVersionFileList.Checksum)
            {
                if (!CurrentVersionFileList.ToBeDeleted.Contains(kvp.Key))
                    CurrentVersionFileList.Checksum.Add(kvp.Key, kvp.Value);
            }

            foreach (KeyValuePair<string, byte[]> kvp in applicationManifest2.CurrentVersionFileList.Checksum)
            {
                CurrentVersionFileList.Checksum.AddOrReplace(kvp.Key, kvp.Value);
            }
        }

        public ApplicationManifest(ApplicationManifest previousManifest, FileList currentVersionFileList)
        {
            ID = Guid.NewGuid();
            CurrentVersionFileList = currentVersionFileList;
            CreationDate = DateTime.UtcNow;
            PreviousManifest = previousManifest;
            LocalHistory = previousManifest?.LocalHistory;
            LocalHistory?.Add(ID);
        }
    }
}
