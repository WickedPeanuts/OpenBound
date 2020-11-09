using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound_Network_Object_Library.FileManagement
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

        public static List<string> ListAllFiles(string directoryRoot, string[] filteredExtension = null)
        {
            List<string> strList = new List<string>();

            foreach (string fileDirectory in GetAllSubdirectories(directoryRoot))
            {
                foreach (string file in Directory.GetFiles(fileDirectory))
                {
                    if (filteredExtension == null)
                    {
                        strList.Add(file);
                    }
                    else
                    {
                        foreach (string ext in filteredExtension)
                        {
                            if (file.Contains(ext))
                            {
                                strList.Add(ext);
                            }
                        }
                    }
                }
            }

            return strList;
        }
    }
}
