using Openbound_Asset_Tools.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Helper
{
    public class ExplorerHelper
    {
        public static void CreateAllFolders()
        {
            Directory.CreateDirectory(Parameters.InputDirectory);
            Directory.CreateDirectory(Parameters.OutputDirectory);
            Directory.CreateDirectory(Parameters.SpritesheetOutputDirectory);
            Directory.CreateDirectory(Parameters.RawOutputDirectory);
            Directory.CreateDirectory(Parameters.ComparisonOutputDirectory);
            Directory.CreateDirectory(Parameters.OutputDirectory);
            Directory.CreateDirectory(Parameters.FixedImageOutputDirectory);
            Directory.CreateDirectory(Parameters.TextureOutputDirectory);
            Directory.CreateDirectory(Parameters.ButtonOutputDirectory);
            Directory.CreateDirectory(Parameters.ContentDirectory);
        }

        public static void OpenDirectory(string folderPath) { 

            if (Directory.Exists(folderPath))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    Arguments = folderPath,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            };

            
        }
        }
    }

