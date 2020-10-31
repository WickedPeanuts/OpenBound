using Openbound_Asset_Tools.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Helper
{
    public class PaintHelper
    {
        public static void OpenPictureFromOutputFolder(string filename)
        {
            try
            {
                //This code does not work on .net core 3.1
                /*ProcessStartInfo procInfo = new ProcessStartInfo();
                procInfo.FileName = ("mspaint.exe");
                procInfo.Arguments = Parameters.SpritesheetOutputDirectory + filename;
                Process.Start(procInfo);*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
