using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.General;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Interface.Interactive.GameList
{
    public class OnlineUserList
    {
        List<Nameplate> nameplateList;
        ScrollBar scrollBar;

        Sprite background;

        Vector2 boxTextArea;

        //Rendering/Updating variables
        int startingRenderingIndex;
        int finalRenderingIndex;


        int selectedIndex;
        int maximumRenderableLines;
        Vector2 elementYOffset;

        /// <summary>
        /// This implementation is very similar to <see cref="TextBox"/>. Except that lines aren't spritetexts but nameplates.
        /// No extra threads are created on this method since its mostly static without need for aditional refreshs.
        /// Another difference is the possibility to remove any field at any time due to its nature.
        /// </summary>
        public OnlineUserList(Vector2 position, Vector2 boxSize, float backgroundAlpha = 0f)
        {
            nameplateList = new List<Nameplate>();

            scrollBar = new ScrollBar(position + new Vector2(boxSize.X, 0), boxSize, 1);
            boxTextArea = boxSize - new Vector2(scrollBar.ElementWidth, 0);
            scrollBar.Disable();
            scrollBar.InstallOnChangeAction(OnBeingDragged);

            background = new Sprite("Interface/TextBox/TextBoxBackground", position, layerDepth: DepthParameter.InterfaceButton);
            background.SetTransparency(backgroundAlpha);
            background.Scale *= boxSize;
        }

        public void OnBeingDragged(object button)
        {
            lock (nameplateList)
            {
                selectedIndex = Math.Max((int)(scrollBar.NormalizedCurrentScrollPercentage * nameplateList.Count) - 1, 0);
            }
        }

        public void AppendNameplate(Player player)
        {
            lock (nameplateList)
            {
                nameplateList.Add(new Nameplate(player, Alignment.Left, default));

                if (maximumRenderableLines == 0)
                {
                    elementYOffset = nameplateList.First().ElementDimensions() * Vector2.UnitY - new Vector2(0, 2);
                    maximumRenderableLines = (int)(boxTextArea.Y / elementYOffset.Y);
                }

                //If the number of text exceeds the maximum capacity, enables the scroll bar
                if (nameplateList.Count >= maximumRenderableLines && !scrollBar.IsEnabled) scrollBar.Enable();

                //If the selected index is the last
                if (selectedIndex + 1 == nameplateList.Count - 1)
                    //Selects the last element
                    selectedIndex++;

                if (scrollBar.IsEnabled)
                    //Keeps the same selected element and just updates the scroll bar to its new position.
                    scrollBar.SetScrollPercentagePosition((float)(selectedIndex + 1) / nameplateList.Count);
            }
        }

        public void RemoveNameplate(Player player)
        {
            lock (nameplateList)
            {
                bool hasRemoved = false;

                //Selects a index, removes the element
                for (int i = 0, removingIndex = 0; i < nameplateList.Count; i++, removingIndex++)
                {
                    if (nameplateList[i].Player.ID == player.ID)
                    {
                        if (removingIndex <= selectedIndex)
                            selectedIndex = Math.Max(0, selectedIndex - 1);

                        nameplateList.RemoveAt(removingIndex);
                        hasRemoved = true;
                        break;
                    }
                }

                if (!hasRemoved) return;

                //If the number of text exceeds the maximum capacity, enables the scroll bar
                if (nameplateList.Count < maximumRenderableLines && scrollBar.IsEnabled) scrollBar.Disable();

                //Keeps the same selected element and just updates the scroll bar to its new position.
                if (scrollBar.IsEnabled) scrollBar.SetScrollPercentagePosition((selectedIndex + 1f) / nameplateList.Count);

                //Updating selected index
                startingRenderingIndex = Math.Max(selectedIndex - maximumRenderableLines + 1, 0);
                finalRenderingIndex = Math.Min(startingRenderingIndex + maximumRenderableLines, nameplateList.Count);
            }
        }

        public void Clear()
        {
            lock (nameplateList)
            {
                nameplateList.Clear();
                selectedIndex = 0;
                scrollBar.Disable();

                startingRenderingIndex = finalRenderingIndex = 0;
            }
        }

        public void Update()
        {
            scrollBar.Update();
            background.UpdateAttatchmentPosition();
            UpdateNameplatePosition();
        }

        public void UpdateNameplatePosition()
        {
            lock (nameplateList)
            {
                if (nameplateList.Count == 0) return;

                //Updating selected index
                startingRenderingIndex = Math.Max(selectedIndex - maximumRenderableLines + 1, 0);
                finalRenderingIndex = Math.Min(startingRenderingIndex + maximumRenderableLines, nameplateList.Count);

                Vector2 startingPosition = background.Position.ToIntegerDomain() + (elementYOffset / 2).ToIntegerDomain();
                
                for(int i = startingRenderingIndex; i < finalRenderingIndex; i++)
                {
                    Nameplate nmp = nameplateList[i];
                    nmp.Update(startingPosition + (i - startingRenderingIndex) * elementYOffset);
                    nmp.ResetTextColor();
                }

                nameplateList[selectedIndex].ReplaceTextColor(nameplateList[selectedIndex].PlayerColor, Color.White);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            background.Draw(null, spriteBatch);
            scrollBar.Draw(spriteBatch);

            //Draw only the necessary elements. The indexes are beingcalculated on UpdateTextPosition method.
            lock(nameplateList)
                for(int i = startingRenderingIndex; i < finalRenderingIndex; i++)
                    nameplateList[i].Draw(spriteBatch);
        }
    }
}
