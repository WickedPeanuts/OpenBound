using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace OpenBound_Network_Object_Library.FileManagement.Versioning
{
    public class FileList
    {
        [JsonIgnore] public List<string> ToBeDownloaded => Checksum.Keys.ToList();
        [JsonIgnore] public List<string> ToBeIgnored { get; set; }
        public List<string> ToBeDeleted;
        public Dictionary<string, byte[]> Checksum;

        public FileList()
        {
            ToBeDeleted = new List<string>();
            ToBeIgnored = new List<string>();
            Checksum = new Dictionary<string, byte[]>();
        }
    }
}
