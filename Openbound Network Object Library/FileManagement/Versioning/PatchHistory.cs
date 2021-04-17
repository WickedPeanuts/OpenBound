using Newtonsoft.Json;
using OpenBound_Network_Object_Library.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OpenBound_Network_Object_Library.FileManagement.Versioning
{
    public class PatchHistory
    {
        public Guid ID;
        
        public FileList FileList;
        public DateTime CreationDate;

        public string PatchVersionName;

        public List<PatchHistory> PatchHistoryList;

        [JsonIgnore] public string PatchHistoryPath =>
            $@"{NetworkObjectParameters.PatchHistoryFilename}-{CreationDate:dd-MM-yyyy}-{ID}{NetworkObjectParameters.PatchHistoryExtension}";

        [JsonIgnore] public string BuildPatchPath =>
            $@"{NetworkObjectParameters.GamePatchFilename}-{CreationDate:dd-MM-yyyy}-{ID}{NetworkObjectParameters.GamePatchExtension}";

        public PatchHistory() {
            PatchHistoryList = new List<PatchHistory>();
        }

        public PatchHistory(PatchHistory patchHistory, FileList fileList, string patchVersionName)
        {
            ID = Guid.NewGuid();
            FileList = fileList;
            PatchVersionName = patchVersionName;
            CreationDate = DateTime.UtcNow;

            if (patchHistory != null)
            {
                PatchHistoryList = patchHistory.PatchHistoryList.ToList();
                PatchHistoryList.Add(patchHistory);
                patchHistory.FileList = null;
                patchHistory.PatchHistoryList = null;
            } else
            {
                PatchHistoryList = new List<PatchHistory>();
            }
        }

        public override string ToString()
        {
            return $"{PatchVersionName}-{ID}";
        }

        public static PatchHistory CreatePatchHistoryInstance(string patchHistoryPath = null)
        {
            if (patchHistoryPath == null)
                return new PatchHistory();

            return ObjectWrapper.DeserializeFile<PatchHistory>(patchHistoryPath);
        }
    }
}
