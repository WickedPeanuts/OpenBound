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

using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OpenBound.GameComponents.Asset
{
    public class AssetManager
    {
        private Dictionary<string, object> assetDictionary;
        private ContentManager contentManager;

        public AssetManager(ContentManager ContentManager)
        {
            contentManager = ContentManager;
            assetDictionary = new Dictionary<string, object>();
        }

        private void LoadAsset<T>(string AssetPath)
        {
            string[] completePath = AssetPath.Split('\\');
            Dictionary<string, object> refDictionary = assetDictionary;

#if DEBUG
            System.Diagnostics.Debug.WriteLine($"Loading Asset: {AssetPath}");
#endif

            for (int i = 0; i < completePath.Length;)
            {
                if (refDictionary.ContainsKey(completePath[i]))
                {
                    if (i < completePath.Length - 1)
                        refDictionary = (Dictionary<string, object>)refDictionary[completePath[i]];

                    i++;
                }
                else
                {
                    if (i < completePath.Length - 1)
                        refDictionary.Add(completePath[i], new Dictionary<string, object>());
                    else
                        refDictionary.Add(completePath[i], contentManager.Load<T>(AssetPath));
                }
            }
        }

        internal void LoadAsset<T>(string[] AssetPaths)
        {
            foreach (string s in AssetPaths) LoadAsset<T>(s);
        }

        internal void LoadAsset<T>(List<string> AssetPaths)
        {
            foreach (string s in AssetPaths) LoadAsset<T>(s);
        }

        internal object RequestAsset(string AssetPath)
        {
            string[] completePath = @AssetPath.Split('/');
            Dictionary<string, object> refDictionary = assetDictionary;

            for (int i = 0; i < completePath.Length - 1; i++)
            {
                if (refDictionary.ContainsKey(completePath[i]))
                {
                    if (i < completePath.Length)
                        refDictionary = (Dictionary<string, object>)refDictionary[completePath[i]];
                }
            }

            if (refDictionary.ContainsKey(completePath.Last()))
                return refDictionary[completePath.Last()];
            else
                return null;
        }
    }
}
