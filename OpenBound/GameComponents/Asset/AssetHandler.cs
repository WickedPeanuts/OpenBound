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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Level.Scene;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.FileManagement;
using System;
using System.Collections.Generic;
using System.IO;

namespace OpenBound.GameComponents.Asset
{
    public class AssetHandler
    {
        private AssetHandler() { }
        private static AssetHandler instance;
        public static AssetHandler Instance
        {
            get
            {
                if (instance == null) instance = new AssetHandler();
                return instance;
            }
        }

        private AssetManager manager;
        private ContentManager contentManager;
        private GraphicsDevice graphicsDevice;

        private string assetBasePath;

        public void Initialize(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphicsDevice;
            manager = new AssetManager(contentManager);
            assetBasePath = $@"{Directory.GetCurrentDirectory()}\{this.contentManager.RootDirectory}\";
        }

        private void AppendFilePaths(ref List<string> filePathList, string currentDirectory)
        {
            List<string> fileList = new List<string>();

            foreach (string subDirectory in IOManager.GetAllSubdirectories($@"{assetBasePath}{currentDirectory}"))
                fileList.AddRange(Directory.GetFiles(subDirectory));

            foreach (string path in fileList)
            {
#if DEBUG
                Console.WriteLine($"ImportedFile: {path}");
#endif

                if (path.Contains(".xnb"))
                {
                    filePathList.Add(
                        path.Replace(assetBasePath, "")
                            .Replace(".xnb", ""));
                }
            }
        }

        public (SpriteFont, FontTextType) RequestFont(FontTextType type, string text)
        {
            if ((type & FontTextType.FONT_FAMILY_MASK) == type)
            {
                return RequestLocalizedFont(type, text);
            }

            return (RequestFont($@"Fonts/{type}"), type);
        }

        public (SpriteFont, FontTextType) RequestLocalizedFont(FontTextType type, string text)
        {
            FontTextType family = type & FontTextType.FONT_FAMILY_MASK;

            if (family <= 0)
            {
                throw new Exception("This method only supports font families");
            }

            if (!FontFamilyMetadata.FontFamilyInstances.TryGetValue(family, out IReadOnlyList<FontTextType> fonts))
            {
                throw new Exception("The given font family does not exist");
            }

            foreach (FontTextType font in fonts)
            {
                SpriteFont sf = RequestFont($@"Fonts/{font}");

                bool containsAllCharacters = true;

                foreach (char c in text)
                {
                    containsAllCharacters &= sf.Characters.Contains(c);
                    if (!containsAllCharacters)
                    {
                        break;
                    }
                }

                if (containsAllCharacters)
                    return (sf, font);
            }

            throw new Exception("Font has glyphs from different alphabets");
        }

        public SpriteFont RequestFont(string path)
        {
            SpriteFont rFont = (SpriteFont)manager.RequestAsset(path);

            if (rFont == null)
                throw new Exception();

            return rFont;
        }

        public Texture2D RequestTexture(string path)
        {
            Texture2D rAsset = (Texture2D)manager.RequestAsset(path);

            if (rAsset == null)
            {
#if DEBUG
                Console.WriteLine($"Resource not found: {path}");
#endif
                throw new Exception();
            }

            return rAsset;
        }

        public Texture2D RequestTextureCopy(string path)
        {
            Texture2D rAsset = (Texture2D)manager.RequestAsset(path);

            if (rAsset == null)
            {
#if DEBUG
                Console.WriteLine($"Resource not found: {path}");
#endif
                throw new Exception();
            }

            //Performing the copy
            Color[] colorArray = new Color[rAsset.Width * rAsset.Height];
            rAsset.GetData(colorArray);

            Texture2D newTexture = new Texture2D(graphicsDevice, rAsset.Width, rAsset.Height);
            newTexture.SetData(colorArray);

            return newTexture;
        }

        public Texture2D RequestTextureCopy(string path, Color color)
        {
            Texture2D rAsset = (Texture2D)manager.RequestAsset(path);

            if (rAsset == null)
            {
#if DEBUG
                Console.WriteLine($"Resource not found: {path}");
#endif
                throw new Exception();
            }

            //Performing the copy
            Color[] colorArray = new Color[rAsset.Width * rAsset.Height];
            rAsset.GetData(colorArray);

            for (int i = 0; i < colorArray.Length; i++)
                colorArray[i] = colorArray[i].TintPixel(color);

            Texture2D newTexture = new Texture2D(graphicsDevice, rAsset.Width, rAsset.Height);
            newTexture.SetData(colorArray);

            return newTexture;
        }

        public Texture2D RequestTextureCopy(Texture2D texture2D, int newWidth, int newHeight, int startingIndexX, int startingIndexY, int offsetX, int offsetY)
        {
            Color[][] originalData = texture2D.GetData();
            Color[][] newOriginalData = originalData.ExtractMatrixSlice(startingIndexX, texture2D.Width - startingIndexX, startingIndexY, texture2D.Height - startingIndexY);
            Color[][] newColorMatrix = Helper.CreateMatrix(newWidth, newHeight, Color.Transparent);

            newColorMatrix.ApplyMatrixSlice(newOriginalData, offsetX, offsetY);
            Texture2D newTexture = new Texture2D(graphicsDevice, newWidth, newHeight);
            newTexture.SetData(newColorMatrix.ConvertTo1D());

            return newTexture;
        }

        public Texture2D CreateAsset(int Width, int Height)
        {
            return new Texture2D(graphicsDevice, Width, Height);
        }

        public Song RequestSong(string path)
        {
            Song rAsset = (Song)manager.RequestAsset(path);

            if (rAsset == null)
            {
#if DEBUG
                Console.WriteLine($"Resource not found: {path}");
#endif
                throw new Exception();
            }

            return rAsset;
        }

        public SoundEffect RequestSoundEffect(string path)
        {
            SoundEffect rAsset = (SoundEffect)manager.RequestAsset(path);

            if (rAsset == null)
            {
#if DEBUG
                Console.WriteLine($"Resource not found: {path}");
#endif
                //throw new Exception();
            }

            return rAsset;
        }

        //Load functions
        public void LoadSpriteFonts()
        {
            List<string> sList = new List<string>();
            AppendFilePaths(ref sList, @"Fonts");
            manager.LoadAsset<SpriteFont>(sList);
        }

        public void LoadAllAssets()
        {
            LoadSpriteFonts();

            // 2D Textures
            List<string> sList = new List<string>();
            AppendFilePaths(ref sList, $@"Debug");
            AppendFilePaths(ref sList, $@"Graphics");
            AppendFilePaths(ref sList, $@"Interface");
            AppendFilePaths(ref sList, $@"Misc");
            manager.LoadAsset<Texture2D>(sList);

            //Sound Effects
            sList.Clear();
            AppendFilePaths(ref sList, $@"Audio\SFX");
            manager.LoadAsset<SoundEffect>(sList);

            //Audio
            sList.Clear();
            AppendFilePaths(ref sList, $@"Audio\Music");
            manager.LoadAsset<Song>(sList);

            /*
            LoadMenuAssets();
            LoadMapAssets();
            LoadTankAssets();
            LoadCursorAssets();
            LoadHUDAssets();
            LoadDebugAssets();
            LoadHealthAsset();
            */
        }
    }
}
