using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openbound_Asset_Tools.Utils
{
    public class SpritefontRangeBuilder
    {
        //This code is terrible and I am ashamed of comitting it.

        /// <summary>
        /// Receives a list of valid font elements and spits out on the console the correct character regions to be placed inside .spritefont file
        /// </summary>
        /// <param name="validFontIndexes"></param>
        public static void BuildSpritefontRange(int[] validFontIndexes = null)
        {
            string[] str = File.ReadAllText("C:\\Users\\Carlos\\source\\repos\\OpenBound\\GunboundImageFix\\Resources\\FontAwesome5.3ValidElements.txt").Split(',');
            
            validFontIndexes = new int[validFontIndexes.Count()];

            for (int i = 0; i < str.Length; i++)
                validFontIndexes[i] = int.Parse(str[i]);

            List<(int, int)> tupleList = new List<(int, int)>();

            for (int i = 0; i < validFontIndexes.Length; i++)
            {
                int index = validFontIndexes[i];
                int pIndex = validFontIndexes[i];
                while (true)
                {
                    if (validFontIndexes.Contains(index + 1))
                    {
                        index++;
                        i = validFontIndexes.ToList().IndexOf(index);
                    }
                    else
                    {
                        break;
                    }
                }
                tupleList.Add((pIndex, index));
            }

            tupleList.ForEach((yy) => Console.WriteLine($"<CharacterRegion><Start>&#{yy.Item1};</Start><End>&#{yy.Item2};</End></CharacterRegion>"));

            Console.ReadKey();
        }
    }
}
