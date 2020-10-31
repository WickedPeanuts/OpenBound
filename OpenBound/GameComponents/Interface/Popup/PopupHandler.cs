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
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenBound.GameComponents.Interface.Popup
{
    public class PopupHandler
    {
        private static List<PopupMenu> popupList;
        private static List<PopupMenu> toBeRemovedPopupList;
        private static List<PopupMenu> toBeAddedPopupList;

        public static PopupGameOptions PopupGameOptions { get; protected set; }
        public static PopupCreateGame PopupCreateGame { get; protected set; }

        public static void Initialize()
        {
            popupList = new List<PopupMenu>();
            toBeAddedPopupList = new List<PopupMenu>();
            toBeRemovedPopupList = new List<PopupMenu>();

            popupList.Add(PopupGameOptions = new PopupGameOptions(Vector2.Zero));
        }

        public static void Update(GameTime gameTime)
        {
            popupList.ForEach((x) => x.Update(gameTime));

            toBeRemovedPopupList.ForEach((x) => popupList.Remove(x));
            toBeRemovedPopupList.Clear();

            popupList.AddRange(toBeAddedPopupList);
            toBeAddedPopupList.Clear();
        }

        public static PopupMenu RequestInstance(Type type)
        {
            return popupList.FirstOrDefault((x) => x.GetType() == type);
        }

        public static void Add(PopupMenu popupMenu)
        {
            if (popupMenu.GetType() != typeof(PopupAlertMessage) &&
                popupList.Find((x) => x.GetType() == popupMenu.GetType()) != null) return;

            toBeAddedPopupList.Add(popupMenu);
        }

        public static void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            popupList.ForEach((x) => x.Draw(gameTime, spriteBatch));
        }

        internal static void Remove(PopupMenu popupMenu)
        {
            toBeRemovedPopupList.Add(popupMenu);
        }

        public void Clear()
        {
            popupList.Clear();
            toBeAddedPopupList.Clear();
            toBeRemovedPopupList.Clear();
        }
    }
}
