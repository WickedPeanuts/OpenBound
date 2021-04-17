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
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.GameComponents.Level.Scene;
using OpenBound.GameComponents.Asset;
using System;
using System.Linq;
using OpenBound.GameComponents.Audio;

namespace OpenBound
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public static Game Instance;

        SceneHandler sceneHandler;
        GraphicsDeviceManager graphics;
        
        public Game1()
        {
            Instance = this;

            graphics = new GraphicsDeviceManager(this);
            graphics.PreparingDeviceSettings += GraphicsDeviceSettings;
            //Mouse.SetPosition(Parameter.ScreenResolutionWidth/2, Parameter.ScreenResolutionHeight/2);
            Content.RootDirectory = "Content";

#if DEBUG
            IsMouseVisible = true;
#endif

            //Lock fps at 60
            IsFixedTimeStep = true;

            //Creating the scene handler
            sceneHandler = SceneHandler.Instance;

            Window.AllowAltF4 = false;
        }

        private void GraphicsDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            if (Parameter.IsFullScreen) {
                graphics.IsFullScreen = true;
                graphics.HardwareModeSwitch = false;

                Parameter.ScreenResolution = new Vector2(
                    e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Width,
                    e.GraphicsDeviceInformation.Adapter.CurrentDisplayMode.Height);

                Parameter.ScreenCenter = Parameter.ScreenResolution / 2;
            }

            graphics.PreferMultiSampling = Parameter.IsAntiAliasingOn;
            graphics.SynchronizeWithVerticalRetrace = Parameter.IsVSyncOn;
            
            e.GraphicsDeviceInformation.PresentationParameters.MultiSampleCount = 8;
        }

        private void GraphicsUpdateResolutionSettings()
        {
            graphics.GraphicsDevice.PresentationParameters.BackBufferWidth =
                graphics.PreferredBackBufferWidth =
                (int)Parameter.ScreenResolution.X;
            graphics.GraphicsDevice.PresentationParameters.BackBufferHeight =
                graphics.PreferredBackBufferHeight =
                (int)Parameter.ScreenResolution.Y;
            
            //Updates the game's viewport
            graphics.GraphicsDevice.Viewport = new Viewport(0, 0, (int)Parameter.ScreenResolution.X, (int)Parameter.ScreenResolution.Y, 0, 1);

            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            base.Initialize();

            GraphicsUpdateResolutionSettings();

            AudioHandler.Initialize();
            sceneHandler.Initialize(GraphicsDevice);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
            AssetHandler.Instance.Initialize(Content, GraphicsDevice);
            AssetHandler.Instance.LoadAllAssets();

            MetadataManager.Instance.Initialize();
            MetadataManager.Instance.LoadAssetMetadata();

            Parameter.IsLoadingGameAssets = false;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Environment.Exit(0);
            }

            sceneHandler.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="GameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime GameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            // TODO: Add your drawing code here
            sceneHandler.Draw(GameTime);

            base.Draw(GameTime);
        }
    }
}
