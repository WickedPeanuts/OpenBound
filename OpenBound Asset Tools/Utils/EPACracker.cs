using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenBound_Image_Fix.Utils
{
    class EPACracker
    {
        public static void CrackEPA(string path)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                int animationInstanceNumber = reader.ReadInt32();

                for (int i = 0; i < animationInstanceNumber; i++)
                {
                    int animationInstanceNameLength = reader.ReadInt32();
                    string animationInstanceName = "";

                    for (int j = 0; j < animationInstanceNameLength; j++)
                        animationInstanceName += (char)reader.ReadSByte();

                    Console.WriteLine("Animation name: " + animationInstanceName);

                    bool shouldRepeatAnimation = reader.ReadByte() != 0;

                    int animationFrameNumber = reader.ReadInt32();

                    Console.Write("Frames: ");
                    for (int k = 0; k < animationFrameNumber; k++)
                    {
                        // FRAMES
                        int frameIndex = reader.ReadInt32();
                        Console.Write(" " + frameIndex);
                    }

                    Console.WriteLine();
                    Console.Write("Duration: ");
                    for (int l = 0; l < animationFrameNumber; l++)
                    {
                        /// Duration each
                        int frameIndex = reader.ReadInt32();
                        Console.Write(" " + frameIndex);
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
