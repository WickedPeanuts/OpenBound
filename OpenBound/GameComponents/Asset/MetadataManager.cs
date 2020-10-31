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

using OpenBound_Network_Object_Library.Common;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OpenBound.GameComponents.Asset
{
    /// <summary>
    /// This method reads all documents inside ContentMetadata folder and saves the metadata files
    /// on <see cref="ElementMetadata"/>. <see cref="Initialize"/> and <see cref="LoadAssetMetadata"/>
    /// must be called before using any elements whithin.
    /// </summary>
    public class MetadataManager
    {
        public static Dictionary<string, object> ElementMetadata = new Dictionary<string, object>();
        
        public static Dictionary<Gender, 
            Dictionary<AvatarCategory, 
                Dictionary<int, AvatarMetadata>>> AvatarMetadataDictionary =
            new Dictionary<Gender, 
                Dictionary<AvatarCategory, 
                    Dictionary<int, AvatarMetadata>>>();

        private static MetadataManager instance;
        public static MetadataManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new MetadataManager();

                return instance;
            }
        }

        public MetadataManager() { }

        string metadataBasePath;

        //ideally the game should fetch the information from a browser
        string downloadedMetadataBasePath;

        public void Initialize()
        {
            metadataBasePath = $@"{Directory.GetCurrentDirectory()}\ContentMetadata\";
            downloadedMetadataBasePath = $@"{Directory.GetCurrentDirectory()}\";
        }

        public void LoadAssetMetadata()
        {
            //Mobile metadata
            LoadMetadata<List<int[]>>(metadataBasePath, "Mobile");

            //Avatar metadata
            LoadMetadata<List<AvatarMetadata>>(downloadedMetadataBasePath, "DatabaseSeed");
            LoadAvatarMetadata();
        }
        
        /// <summary>
        /// Saves the AvatarMetadata elements into <see cref="AvatarMetadataDictionary"/>.
        /// </summary>
        private void LoadAvatarMetadata()
        {
            ElementMetadata["DatabaseSeed/AvatarMetadata"] = ((List<AvatarMetadata>)ElementMetadata["DatabaseSeed/AvatarMetadata"]).OrderBy((x) => x.AvatarCategory).ThenBy((y) => y.ID).ToList();

            AvatarMetadataDictionary.Add(Gender.Male,   new Dictionary<AvatarCategory, Dictionary<int, AvatarMetadata>>());
            AvatarMetadataDictionary.Add(Gender.Female, new Dictionary<AvatarCategory, Dictionary<int, AvatarMetadata>>());

            foreach(AvatarCategory category in (AvatarCategory[])Enum.GetValues(typeof(AvatarCategory)))
            {
                AvatarMetadataDictionary[Gender.Male].Add(category, new Dictionary<int, AvatarMetadata>());
                AvatarMetadataDictionary[Gender.Female].Add(category, new Dictionary<int, AvatarMetadata>());
            }

            foreach (AvatarMetadata am in (List<AvatarMetadata>)ElementMetadata["DatabaseSeed/AvatarMetadata"])
            {
                if (am.Gender != Gender.Unissex)
                    AvatarMetadataDictionary[am.Gender][am.AvatarCategory].Add(am.ID, am);
                else
                {
                    AvatarMetadataDictionary[Gender.Male][am.AvatarCategory].Add(am.ID, am);
                    AvatarMetadataDictionary[Gender.Female][am.AvatarCategory].Add(am.ID, am);
                }
            }
        }

        private void LoadMetadata<T>(string basePath, string metadataPath)
        {
            List<string> filePaths = new List<string>();

            string basePivotPath = $"{basePath}{metadataPath}";
            IOManager.GetAllSubdirectories(basePivotPath).ForEach((x) => filePaths.AddRange(Directory.GetFiles(x)));

            foreach (string str in filePaths)
            {
#if DEBUG
                Console.WriteLine(str);
#endif

                string fileContent = File.ReadAllText(str);

                string dicPath = str.Replace($"{basePivotPath}\\", "").Replace("\\","/").Split('.')[0];
                ElementMetadata.Add($@"{metadataPath}/{dicPath}", ObjectWrapper.DeserializeRequest<T>(fileContent));
            }
        }
    }
}
