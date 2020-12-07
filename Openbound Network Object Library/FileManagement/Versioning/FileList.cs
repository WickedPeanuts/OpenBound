using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OpenBound_Network_Object_Library.FileManagement.Versioning
{
    public class FileList
    {
        [JsonIgnore] public List<string> ToBeDownloaded { get; set; }
        [JsonIgnore] public List<string> ToBeIgnored { get; set; }
        [JsonIgnore] public List<string> ToBeDeleted { get; set; }

        public Dictionary<string, byte[]> Checksum;

        public FileList()
        {
            ToBeDeleted = new List<string>();
            ToBeIgnored = new List<string>();
            ToBeDownloaded = new List<string>();
            Checksum = new Dictionary<string, byte[]>();
        }
    }
}
