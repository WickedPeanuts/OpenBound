using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenBound.GameComponents.Interface.Interactive
{
    public struct TabButtonParameter
    {
        public string ButtonText;
        public Action<object> OnClick;
        public TabButtonParameter(string buttonText, Action<object> onClick) {
            ButtonText = buttonText;
            OnClick = onClick;
        }
    }

    public class TabButtonList
    {
        public List<Button> buttonList;
        public List<SpriteText> spriteTextList;

        int currentTabIndex;

        public TabButtonList(Vector2 position, List<TabButtonParameter> tabButtonParameterList)
        {
            buttonList = new List<Button>();
            spriteTextList = new List<SpriteText>();

            Vector2 basePos = position;

            foreach(TabButtonParameter tbp in tabButtonParameterList)
            {
                SpriteText st = new SpriteText(FontTextType.Consolas10, tbp.ButtonText, Color.Gray, Alignment.Center, DepthParameter.InterfacePopupButtonIcon, default, Color.Black);
                st.PositionOffset = basePos;// + (-st.MeasureSize * Vector2.UnitY / 3f);
                spriteTextList.Add(st);

                Button b = new Button(ButtonType.AvatarTabIndex, DepthParameter.InterfacePopupButtons, (o) => OnClick(o, tbp.OnClick, st), basePos);
                buttonList.Add(b);

                basePos += new Vector2(80, 0);
            }

            SelectTabButton(0);
        }

        public void Enable()
        {
            SelectTabButton(currentTabIndex);
        }

        public void Disable()
        {
            DeselectTabButton(currentTabIndex);
        }

        public void RealocateElements(Vector2 delta)
        {
            buttonList.ForEach((x) => x.ButtonOffset += delta);
            spriteTextList.ForEach((x) => x.PositionOffset += delta);
        }

        public void SelectTabButton(int i)
        {
            spriteTextList[i].PositionOffset -= spriteTextList[i].MeasureSize * Vector2.UnitY / 3f;
            spriteTextList[i].Color = spriteTextList[i].BaseColor = Color.White;
            buttonList[i].Disable();
        }

        public void DeselectTabButton(int i)
        {
            spriteTextList[i].PositionOffset += spriteTextList[i].MeasureSize * Vector2.UnitY / 3f;
            spriteTextList[i].Color = spriteTextList[i].BaseColor = Color.Gray;
            buttonList[i].Enable();
        }

        public void OnClick(object sender, Action<object> clickAction, SpriteText spriteText)
        {
            for(int i = 0; i < buttonList.Count; i++)
            {
                if (buttonList[i] == sender)
                {
                    SelectTabButton(i);
                    currentTabIndex = i;
                }
                else if (!buttonList[i].IsEnabled)
                {
                    DeselectTabButton(i);
                }
            }

            clickAction?.Invoke(sender);
        }

        public void Update()
        {
            buttonList.ForEach((x) => x.Update());
            spriteTextList.ForEach((x) => x.UpdateAttatchedPosition());
        }

        public void UpdateAttatchedPosition()
        {
            buttonList.ForEach((x) => x.UpdateAttatchedPosition());
            spriteTextList.ForEach((x) => x.UpdateAttatchedPosition());
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            buttonList.ForEach((x) => x.Draw(gameTime, spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
        }
    }
}
