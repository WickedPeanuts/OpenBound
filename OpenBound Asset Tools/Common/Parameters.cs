/* 
 * Copyright (C) 2020, Carlos H.M.S. <carlos_judo@hotmail.com>
 * This file is part of OpenBound.
 * OpenBound is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of the License, or(at your option) any later version.
 * 
 * OpenBound is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with OpenBound. If not, see http://www.gnu.org/licenses/.
 */

using System.IO;
using System.Threading;

namespace Openbound_Asset_Tools.Common
{
    public class Parameters
    {
        public const string PivotFileHeader = "#ID,PivotX,PivotY,RiderPivotX,RiderPivotY,PivotX+RiderPivotX,PivotY+RiderPivotY";

        public static string BaseDirectory { get; } = $@"{Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName}\";

        //Input Directories
        public static string InputDirectory { get; } =
            BaseDirectory + @"Input\";

        //Output Directories
        public static string OutputDirectory { get; } =
            BaseDirectory + @"Output\";
        public static string SpritesheetOutputDirectory { get; } =
            OutputDirectory + @"Spritesheet\";
        public static string RawOutputDirectory { get; } =
            OutputDirectory + @"Raw\";
        public static string ComparisonOutputDirectory { get; } =
            OutputDirectory + @"Comparison\";
        public static string FixedImageOutputDirectory { get; } = 
            OutputDirectory + @"FixedImage\";
        public static string TextureOutputDirectory { get; } =
            OutputDirectory + @"Texture\";

        public static string ButtonOutputDirectory { get; } =
            OutputDirectory + @"Button\";

        //Content Directory
        public static string ContentDirectory { get; } =
            BaseDirectory + @"Files\";
    }
}
