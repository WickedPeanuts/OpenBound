using Openbound_Asset_Tools.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Utils
{
    class PivotFileManager
    {
        public static void FixPivotFile()
        {
            List<int[]> list = FileImportManager.ReadPivotFile();

            Console.WriteLine("Offset index: ");
            int offsetIndex = int.Parse(Console.ReadLine());

            List<int[]> newValues = new List<int[]>();
            Console.WriteLine("Replacing index (separated by commas): ");
            Console.ReadLine().Split(',').ToList().ForEach((x) =>
            {
                newValues.Add(list[int.Parse(x)]);
            });

            list.InsertRange(offsetIndex, newValues);

            for(int i = 0; i < list.Count; i++)
            {
                list[i][0] = i;
                Console.WriteLine(list[i][0] + "," + list[i][1] + "," + list[i][2]);
            }
        }
    }
}
