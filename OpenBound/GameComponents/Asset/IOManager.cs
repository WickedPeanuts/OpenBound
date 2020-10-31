using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Asset
{
    public class IOManager
    {
        public static List<string> GetAllSubdirectories(string directoryRoot)
        {
            List<string> dirSubdirectories = new List<string>();
            dirSubdirectories.Add(directoryRoot);
            AppendAllSubdirectoriesIntoList(dirSubdirectories, directoryRoot);
            return dirSubdirectories;
        }

        private static void AppendAllSubdirectoriesIntoList(List<string> directoryList, string directoryRoot)
        {
            foreach (string subDirectory in Directory.GetDirectories(directoryRoot))
            {
                directoryList.Add(subDirectory);
                AppendAllSubdirectoriesIntoList(directoryList, subDirectory);
            }
        }
    }
}
