using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Interface.Interactive;
using OpenBound.GameComponents.Interface.Interactive.Misc;
using OpenBound.GameComponents.Level.Scene;
using System;

namespace OpenBound.GameComponents.Interface.General
{
    /// <summary>
    /// Scrollbar interface element. This class is hardcoded for a single button type. It can be attatch it to any component element
    /// </summary>
    public class ScrollBar
    {
        //Scroll buttons
        Button scrollButtonUp, scrollButtonDown;
        Sprite scrollBackground;

        //Scroll Bar
        Sprite scrollBar;
        TransparentButton scrollBarTB;

        Vector2 position, boxSize;

        //Maximum and minimum possible pixel for the scroll bar
        float maxScroll, minScroll;

        float scrollSize;

        
        public bool IsEnabled { get; private set; }

        /// <summary>
        /// Values ranges from 0 to 1.
        /// </summary>
        public float NormalizedCurrentScrollPercentage { get; private set; }

        //Debug Variables
#if DEBUG
        DebugLine dL1 = new DebugLine(Color.Red);
        DebugLine dL2 = new DebugLine(Color.Red);
        DebugCrosshair d1 = new DebugCrosshair(Color.Blue);
        DebugCrosshair d2 = new DebugCrosshair(Color.Blue);
        DebugCrosshair d3 = new DebugCrosshair(Color.Blue);
#endif

        public ScrollBar(Vector2 position, Vector2 boxSize, float scrollBackgroundAlpha)
        {
            this.position = position;
            this.boxSize = boxSize;

            scrollButtonUp = new Button(ButtonType.ScrollBarUp, DepthParameter.InterfaceButtonAnimatedIcon, default, position);
            scrollButtonUp.ButtonOffset += new Vector2(0, scrollButtonUp.ButtonSprite.Pivot.Y - 1);
            Vector2 pivotOffset = scrollButtonUp.ButtonSprite.Pivot * Vector2.UnitX;
            scrollButtonUp.ButtonSprite.Pivot += pivotOffset;

            scrollButtonDown = new Button(ButtonType.ScrollBarDown, DepthParameter.InterfaceButtonAnimatedIcon, default, position + boxSize * Vector2.UnitY);
            scrollButtonDown.ButtonOffset -= new Vector2(0, scrollButtonDown.ButtonSprite.Pivot.Y - 1);
            scrollButtonDown.ButtonSprite.Pivot += pivotOffset;

            scrollBackground = new Sprite("Interface/TextBox/ScrollBarBackground", position + new Vector2(0, 1), DepthParameter.InterfaceButtonIcon);
            scrollBackground.Scale = new Vector2(1, boxSize.Y - 2);
            scrollBackground.Pivot += pivotOffset;
            scrollBackground.SetTransparency(scrollBackgroundAlpha);

            scrollBar = new Sprite("Interface/TextBox/ScrollBarBlock", scrollButtonDown.ButtonOffset, layerDepth: DepthParameter.InterfaceButtonAnimatedIcon, sourceRectangle: new Rectangle(0, 0, 25, 11));
            scrollBar.Pivot = new Vector2(12.5f, 5.5f) + pivotOffset;

            scrollBarTB = new TransparentButton((scrollButtonUp.ButtonOffset + scrollButtonDown.ButtonOffset) / 2, new Rectangle(0, 0, scrollButtonUp.ButtonSprite.SourceRectangle.Width, (int)boxSize.Y - scrollButtonUp.ButtonSprite.SourceRectangle.Height * 2 + 2));
            scrollBarTB.ButtonSprite.Pivot += scrollBarTB.ButtonSprite.Pivot * Vector2.UnitX;

            scrollBarTB.OnBeingDragged += OnBeingDraggedAction;
            scrollBarTB.OnBeingReleased += OnBeingReleasedAction;

#if DEBUG
            DebugHandler.Instance.Add(dL1);
            DebugHandler.Instance.Add(dL2);
            DebugHandler.Instance.Add(d1);
            DebugHandler.Instance.Add(d2);
            DebugHandler.Instance.Add(d3);
#endif

            minScroll = scrollButtonUp.ButtonOffset.Y + scrollButtonUp.ButtonSprite.Pivot.Y + 5;
            maxScroll = scrollButtonDown.ButtonOffset.Y - scrollButtonDown.ButtonSprite.Pivot.Y - 5;
            scrollSize = Math.Abs(minScroll) + Math.Abs(maxScroll);

            scrollButtonUp.OnBeingDragged += (b) => { UpdateScrollPosition(scrollSize / 100f); };
            scrollButtonDown.OnBeingDragged += (b) => { UpdateScrollPosition(-scrollSize / 100f); };

            UpdateScrollPosition(float.MaxValue);
            
            IsEnabled = true;
        }

        /// <summary>
        /// Returns the element width
        /// </summary>
        public int ElementWidth => scrollButtonUp.ButtonSprite.SourceRectangle.X + 5;

        #region Action Handlers
        public void OnBeingDraggedAction(object button)
        {
            SetScrollPosition(Cursor.Instance.CurrentFlipbook.Position.Y + GameScene.Camera.CameraOffset.Y);
            scrollBar.SourceRectangle = new Rectangle(0, 11, 25, 11);
        }

        public void OnBeingReleasedAction(object button)
        {
            scrollBar.SourceRectangle = new Rectangle(0, 0, 25, 11);
        }

        /// <summary>
        /// Install an action into <see cref="scrollBarTB"/>, <see cref="scrollButtonUp"/> and <see cref="scrollButtonDown"/>.
        /// </summary>
        /// <param name="action">The <see cref="object"/> on the action is the button element.</param>
        public void InstallOnChangeAction(Action<object> action)
        {
            scrollBarTB.OnBeingDragged += action;
            scrollButtonUp.OnBeingDragged += action;
            scrollButtonDown.OnBeingDragged += action;
        }
        #endregion

        /// <summary>
        /// Sets the percentage to a specified amount.
        /// </summary>
        /// <param name="newPosition">Values ranges from 0 (closeer to upper button) to 1 (closer to bottom button).</param>
        public void SetScrollPercentagePosition(float newPosition)
        {
            scrollBar.PositionOffset = new Vector2(scrollBar.PositionOffset.X, Math.Min(minScroll, maxScroll) + scrollSize * newPosition);
            CalculateNormalizedPosition();
        }

        /// <summary>
        /// Sets the scroll position into a interface-relative parameter.
        /// </summary>
        /// <param name="newPosition">Values ranges from scrollButtonUp.ButtonOffset.Y to scrollButtonDown.ButtonOffset.Y</param>
        public void SetScrollPosition(float newPosition)
        {
            scrollBar.PositionOffset = new Vector2(scrollBar.PositionOffset.X, newPosition);
            CalculateNormalizedPosition();
        }

        /// <summary>
        /// Adds a value (pixel-based) into the current scroll position.
        /// </summary>
        /// <param name="newOffset">Positive values makes the bar go up, negative values makes it go down</param>
        public void UpdateScrollPosition(float newOffset)
        {
            scrollBar.PositionOffset -= new Vector2(0, newOffset);
            CalculateNormalizedPosition();
        }

        /// <summary>
        /// Calculates the new position and updates value of <see cref="NormalizedCurrentScrollPercentage"/>.
        /// </summary>
        private void CalculateNormalizedPosition()
        {
            float min, max;

            if (minScroll > maxScroll)
            {
                max = minScroll;
                min = maxScroll;
            }
            else
            {
                max = maxScroll;
                min = minScroll;
            }

            scrollBar.PositionOffset = new Vector2(scrollBar.PositionOffset.X, MathHelper.Clamp(scrollBar.PositionOffset.Y, min, max));

            //Calculate the normalized (0..1) current scrolled percentage
            NormalizedCurrentScrollPercentage = (scrollBar.PositionOffset.Y - min) / (max - min);
        }

        /// <summary>
        /// Disables the interface element.
        /// </summary>
        public void Disable()
        {
            if (!IsEnabled) return;

            IsEnabled = false;
            scrollButtonUp.Disable();
            scrollButtonDown.Disable();
            scrollBar.SetTransparency(0);
            scrollBarTB.Disable();
        }

        /// <summary>
        /// Enables the interface element.
        /// </summary>
        public void Enable()
        {
            if (IsEnabled) return;

            IsEnabled = true;
            scrollButtonUp.Enable();
            scrollButtonDown.Enable();
            scrollBar.SetTransparency(1);
            scrollBarTB.Enable();
        }

        public void Update()
        {
            scrollButtonUp.Update();
            scrollButtonDown.Update();

            scrollBackground.UpdateAttatchmentPosition();

            scrollBar.UpdateAttatchmentPosition();
            scrollBarTB.Update();

#if DEBUG
            dL1.Update(position - GameScene.Camera.CameraOffset, position + boxSize * Vector2.UnitY - GameScene.Camera.CameraOffset);
            dL2.Update(position - GameScene.Camera.CameraOffset + new Vector2(5, 0), position - GameScene.Camera.CameraOffset + new Vector2(5, 300));
            d1.Update(scrollButtonUp.ButtonSprite.Position);
            d2.Update(scrollButtonDown.ButtonSprite.Position);
            d3.Update(scrollBar.Position);
#endif
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            scrollButtonUp.Draw(null, spriteBatch);
            scrollButtonDown.Draw(null, spriteBatch);
            scrollBackground.Draw(null, spriteBatch);
            scrollBar.Draw(null, spriteBatch);
        }
    }
}
