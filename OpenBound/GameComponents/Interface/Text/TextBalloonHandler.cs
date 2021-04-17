using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Extension;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound.GameComponents.Interface.Text
{
    public class TextBalloonHandler
    {
        private static object asyncPadlock;

        public static List<TextBalloon> toBeRemovedTextBalloonList;
        public static Dictionary<Mobile, TextBalloon> textBalloonDictionary;

        static TextBalloonHandler()
        {
            asyncPadlock = new object();
            toBeRemovedTextBalloonList = new List<TextBalloon>();
            textBalloonDictionary = new Dictionary<Mobile, TextBalloon>();
        }

        public static void Update(GameTime gameTime)
        {
            lock (asyncPadlock)
            {
                textBalloonDictionary.ForEachValues((x) => x.Update(gameTime));
                toBeRemovedTextBalloonList.ForEach((x) => textBalloonDictionary.Remove(x.Mobile));
                toBeRemovedTextBalloonList.Clear();
            }
        }

        public static void AsyncAddTextBalloon(Mobile mobile, PlayerMessage playerMessage, int defaultYOffset = 60, float layerDepth = DepthParameter.InterfaceTextBalloonBase)
        {
            lock (asyncPadlock)
            {
                textBalloonDictionary.AddOrReplace(mobile, new TextBalloon(mobile, playerMessage, defaultYOffset, RemoveTextBalloon, layerDepth));
            }
        }

        public static void RemoveTextBalloon(TextBalloon textBalloon)
        {
            toBeRemovedTextBalloonList.Add(textBalloon);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            lock(asyncPadlock)
                textBalloonDictionary.ForEachValues((x) => x.Draw(spriteBatch));
        }
    }
}
