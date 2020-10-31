using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.Common;
using OpenBound.Extension;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Interface.Text;
using OpenBound.GameComponents.Pawn;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Entity.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace OpenBound.GameComponents.Interface.Text
{
    public enum TextBalloonType
    {
        Normal,
    }

    public enum BalloonBorder
    {
        Top, TopLeft, TopRight,
        Bottom, BottomLeft, BottomRight,
        Center, Left, Right, Arrow,
    }

    public enum TextBalloonAnimation
    {
        Spawning,
        SpawningWriting,
        Spawned,
        Vanishing,
        Vanished,
    }

    public class TextBalloon
    {
        private static readonly Dictionary<TextBalloonType, Dictionary<BalloonBorder, Rectangle>> textBalloonPresets
            = new Dictionary<TextBalloonType, Dictionary<BalloonBorder, Rectangle>>()
            {
                {
                    TextBalloonType.Normal,
                    new Dictionary<BalloonBorder, Rectangle>()
                    {
                        { BalloonBorder.TopLeft,     new Rectangle(1,  1, 4,  4) },
                        { BalloonBorder.TopRight,    new Rectangle(6,  1, 4,  4) },
                        { BalloonBorder.BottomLeft,  new Rectangle(1,  6, 4,  4) },
                        { BalloonBorder.BottomRight, new Rectangle(6,  6, 4,  4) },
                        { BalloonBorder.Arrow,       new Rectangle(1, 11, 9, 11) },
                        { BalloonBorder.Left,        new Rectangle(0,  0, 4,  1) },
                        { BalloonBorder.Right,       new Rectangle(5,  0, 4,  1) },
                        { BalloonBorder.Top,         new Rectangle(0,  0, 1,  4) },
                        { BalloonBorder.Bottom,      new Rectangle(0,  5, 1,  4) },
                        { BalloonBorder.Center,      new Rectangle(0,  0, 1,  1) },
                    }
                },
            };

        public Mobile Mobile { get; private set; }
        public Action<TextBalloon> OnVanishBalloon;

        List<Sprite> spriteList;

        Sprite topLeftBorder,    topBorder,    topRightBorder,
               leftBorder,       centerSquare, rightBorder,
               bottomLeftBorder, bottomBorder, bottomRightBorder,
                                 arrow;

        Vector2 balloonPosition;
        Vector2 textSize, balloonDesiredSize, balloonInitialSize, balloonExpandedSize;

        TextBalloonAnimation textBalloonAnimation;
        float expansionFactor;
        float animationTimer;
        float vanishTimer;

        List<SpriteText> spriteTextList;
        string[] completeText;
        int[] typingIndexMatrix;

        int defaultYOffset;


        public TextBalloon(Mobile mobile, PlayerMessage playerMessage, int defaultYOffset, Action<TextBalloon> onVanishBalloon, float depthParameter)
        {
            Mobile = mobile;
            OnVanishBalloon = onVanishBalloon;

            this.defaultYOffset = defaultYOffset;

            TextBalloonType type = TextBalloonType.Normal;

            string path        = $"Interface/InGame/HUD/Blue/TextBalloon/{type}/";
            string framePath   = $"{path}/BalloonFrame";
            string hBorderPath = $"{path}/BalloonFrameHorizontalBorders";
            string vBorderPath = $"{path}/BalloonFrameVerticalBorders";
            string centerPath  = $"{path}/BalloonCenter";

            Dictionary<BalloonBorder, Rectangle> preset = textBalloonPresets[type];

            spriteList = new List<Sprite>();
            spriteList.Add(topLeftBorder     = new Sprite(framePath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.TopLeft]));
            spriteList.Add(topRightBorder    = new Sprite(framePath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.TopRight]));
            spriteList.Add(bottomLeftBorder  = new Sprite(framePath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.BottomLeft]));
            spriteList.Add(bottomRightBorder = new Sprite(framePath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.BottomRight]));
            spriteList.Add(arrow             = new Sprite(framePath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.Arrow]));
            spriteList.Add(topBorder         = new Sprite(vBorderPath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.Top]));
            spriteList.Add(bottomBorder      = new Sprite(vBorderPath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.Bottom]));
            spriteList.Add(leftBorder        = new Sprite(hBorderPath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.Left]));
            spriteList.Add(rightBorder       = new Sprite(hBorderPath, layerDepth: depthParameter, sourceRectangle: preset[BalloonBorder.Right]));
            spriteList.Add(centerSquare      = new Sprite(centerPath, layerDepth: depthParameter - 0.01f, sourceRectangle: preset[BalloonBorder.Center]));

            spriteList.ForEach((x) => x.Pivot = Vector2.Zero);
            arrow.Pivot += new Vector2(arrow.SourceRectangle.Width / 2, 0).ToIntegerDomain();

            spriteTextList = SpriteText.CreatePlayerTextBalloonMessage(mobile.Owner, playerMessage, 20, depthParameter);

            //Fix string size
            textSize = TextSize();
            balloonDesiredSize = textSize + new Vector2(
                preset[BalloonBorder.TopLeft].Width * 2, 
                preset[BalloonBorder.TopLeft].Height * 2 + textSize.Y * (spriteTextList.Count - 1));
            balloonInitialSize = new Vector2(10, 10);
            balloonExpandedSize = balloonDesiredSize - balloonInitialSize;
            completeText = new string[spriteTextList.Count];
            typingIndexMatrix = new int[spriteTextList.Count];

            for (int i = 0; i < spriteTextList.Count; i++)
            {
                completeText[i] = spriteTextList[i].Text;
                spriteTextList[i].Text = "";
            }

            textBalloonAnimation = TextBalloonAnimation.Spawning;
            animationTimer = 0f;
            expansionFactor = 0f;

            //Resetting text transparency
            spriteList.ForEach((x) => x.SetTransparency(0));

            //Updating position
            UpdateBalloonPosition();
        }
        
        public Vector2 TextSize()
        {
            float x = 0;
            foreach (SpriteText st in spriteTextList)
            {
                Vector2 measuredSize = st.MeasureSize;
                x = Math.Max(x, measuredSize.X);
            }

            return new Vector2(x, spriteTextList[0].MeasureSize.Y);
        }

        public void Update(GameTime gameTime)
        {
            UpdateBalloonAnimation(gameTime);

            UpdateBalloonPosition();
            UpdateTextPosition();
        }

        public void UpdateBalloonAnimation(GameTime gameTime)
        {
            switch (textBalloonAnimation)
            {
                case TextBalloonAnimation.Spawning:
                    expansionFactor += (float)gameTime.ElapsedGameTime.TotalSeconds * 3;
                    animationTimer = (float)Math.Sin(expansionFactor);
                    Vector2 newBalloonSize = balloonInitialSize + balloonExpandedSize * animationTimer;

                    if (expansionFactor > MathHelper.PiOver2)
                    {
                        textBalloonAnimation = TextBalloonAnimation.SpawningWriting;
                        newBalloonSize = balloonInitialSize + balloonExpandedSize;
                        expansionFactor = MathHelper.PiOver2;
                        animationTimer = 1;
                    }

                    bottomBorder.Scale = topBorder.Scale = newBalloonSize * Vector2.UnitX + Vector2.UnitY;
                    rightBorder.Scale = leftBorder.Scale = newBalloonSize * Vector2.UnitY + Vector2.UnitX;
                    centerSquare.Scale = Vector2.Ceiling(newBalloonSize) + Vector2.One * 2;

                    //Transparency
                    spriteList.ForEach((x) => x.SetTransparency(animationTimer));
                    break;
                case TextBalloonAnimation.SpawningWriting:
                    animationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (animationTimer >= 0.02f)
                    {
                        animationTimer = 0f;
                        bool hasChanged = false;

                        for (int i = 0; i < typingIndexMatrix.Length; i++)
                        {
                            if (typingIndexMatrix[i] < completeText[i].Length)
                            {
                                spriteTextList[i].Text += completeText[i][typingIndexMatrix[i]];
                                typingIndexMatrix[i]++;
                                hasChanged = true;
                                break;
                            }
                        }

                        if (!hasChanged)
                        {
                            animationTimer = 1f;
                            textBalloonAnimation = TextBalloonAnimation.Spawned;
                        }
                    }
                    break;
                case TextBalloonAnimation.Spawned:
                    vanishTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (vanishTimer >= 5)
                        textBalloonAnimation = TextBalloonAnimation.Vanishing;
                    break;
                case TextBalloonAnimation.Vanishing:
                    animationTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                    spriteList.ForEach((x) => x.SetTransparency(animationTimer));
                    spriteTextList.ForEach((x) => x.SetTransparency((animationTimer - 0.2f) / 2));
                    if (animationTimer <= 0)
                        textBalloonAnimation = TextBalloonAnimation.Vanished;
                    break;
                case TextBalloonAnimation.Vanished:
                    OnVanishBalloon?.Invoke(this);
                    break;
            }
        }

        public void UpdateTextPosition()
        {
            Vector2 middle = topLeftBorder.Position + bottomRightBorder.Position + new Vector2(bottomRightBorder.SourceRectangle.Width, bottomRightBorder.SourceRectangle.Height);
            middle = (middle / 2).ToIntegerDomain();

            Vector2 startingPos = middle - new Vector2(0, spriteTextList.Count * textSize.Y - 3) / 2;

            for (int i = 0; i < spriteTextList.Count; i++)
                spriteTextList[i].Position = startingPos + i * new Vector2(0, textSize.Y + 1);
        }

        public void UpdateBalloonPosition()
        {
            balloonPosition = Mobile.MobileFlipbook.Position - new Vector2(0, defaultYOffset + balloonDesiredSize.Y / 2) - new Vector2(balloonDesiredSize.X / 2, balloonDesiredSize.Y / 2) * (float)Math.Sin(expansionFactor);

            if (Mobile.Facing == Facing.Right)
                arrow.Effect = SpriteEffects.None;
            else
                arrow.Effect = SpriteEffects.FlipHorizontally;

            topLeftBorder.Position = balloonPosition;
            topBorder.Position = topLeftBorder.Position + new Vector2(topLeftBorder.SourceRectangle.Width, 0);
            topRightBorder.Position = topBorder.Position + new Vector2(topBorder.SourceRectangle.Width, 0) * topBorder.Scale;

            leftBorder.Position = topLeftBorder.Position + new Vector2(0, topLeftBorder.SourceRectangle.Height);
            centerSquare.Position = topBorder.Position + new Vector2(0, topLeftBorder.SourceRectangle.Height) - Vector2.One;
            rightBorder.Position = topRightBorder.Position + new Vector2(0, topRightBorder.SourceRectangle.Height);

            bottomLeftBorder.Position = leftBorder.Position + new Vector2(0, leftBorder.SourceRectangle.Height) * leftBorder.Scale;
            bottomBorder.Position = bottomLeftBorder.Position + new Vector2(bottomLeftBorder.SourceRectangle.Width, 0);
            bottomRightBorder.Position = rightBorder.Position + new Vector2(0, rightBorder.SourceRectangle.Height) * rightBorder.Scale;

            arrow.Position = (bottomLeftBorder.Position + bottomRightBorder.Position + new Vector2(bottomRightBorder.SourceRectangle.Width, 0)) / 2;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteList.ForEach((x) => x.Draw(null, spriteBatch));
            spriteTextList.ForEach((x) => x.Draw(spriteBatch));
        }
    }
}
