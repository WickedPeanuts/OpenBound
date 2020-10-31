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
using OpenBound.Common;
using OpenBound.GameComponents.Debug;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound.GameComponents.Interface.SceneTransition;
using OpenBound.GameComponents.Level.Scene.Menu;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenBound.GameComponents.Level.Scene
{
    public enum TransitionEffectType
    {
        RotatingRectangles,
        None,
    }

    public enum SceneType
    {
        GameLogo,
        ServerSelection,
        GameList,
        GameRoom,
        AvatarShop,
        InGame,
        Exit,
        LoadingScreen,
        DebugScene,
    }

    public enum SceneTransitionState
    {
        Ready,
        Transitioning,
    }

    public class SceneHandler
    {
        public GraphicsDevice GraphicsDevice;
        public SpriteBatch SpriteBatch;
        public GameScene CurrentScene;
        public Queue<GameScene> NextScene;
        public MenuTransitionEffect TransitionEffect;

        private Thread sceneTransitionThread;

        public bool IsChangingScene { get; private set; }

        private float transitionWaitTime;
        private float elapsedTime;

        private static SceneHandler instance;
        public static SceneHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SceneHandler();
                }

                return instance;
            }
        }

        private SceneHandler()
        {
            NextScene = new Queue<GameScene>();
        }

        public void Initialize(GraphicsDevice GraphicsDevice)
        {
            InputHandler.Initialize();

            this.GraphicsDevice = GraphicsDevice;

#if !DEBUGSCENE
            CurrentScene = new GameLogo();
#else
            CurrentScene = new DebugScene();
#endif
            CurrentScene.OnSceneIsActive();

            SpriteBatch = new SpriteBatch(GraphicsDevice);
            CurrentScene.Initialize(GraphicsDevice, SpriteBatch);
            IsChangingScene = false;
        }

        public void ChangeScene()
        {
            lock (NextScene)
            {
                if (NextScene.Count > 0 && !sceneTransitionThread.IsAlive)
                {
                    CurrentScene.Dispose();
                    CurrentScene = NextScene.Dequeue();
                    CurrentScene.OnSceneIsActive();
                    IsChangingScene = false;
                }
            }
        }

        public void CloseGame()
        {
            RequestSceneChange(SceneType.Exit, TransitionEffectType.RotatingRectangles, Game1.Instance.Exit);
        }

        private GameScene BuildNextScene(SceneType SceneType, object SceneParam)
        {
            if (SceneType == SceneType.GameLogo)
                return new GameLogo();
            else if (SceneType == SceneType.ServerSelection)
                return new ServerSelection();
            else if (SceneType == SceneType.GameList)
                return new GameList();
            else if (SceneType == SceneType.AvatarShop)
                return new AvatarShop();
            else if (SceneType == SceneType.GameRoom)
                return new GameRoom();
            else if (SceneType == SceneType.LoadingScreen)
                return new LoadingScreen();
            else if (SceneType == SceneType.InGame)
                return new LevelScene();
            else if (SceneType == SceneType.DebugScene)
                return new DebugScene();

            return null;
        }

        private MenuTransitionEffect BuildTransition(TransitionEffectType transitionEffectType, Action action)
        {
            switch (transitionEffectType)
            {
                case (TransitionEffectType.RotatingRectangles):
                    return new RotatingRectangles(action);
                default:
                    action();
                    return null;
            }
        }

        public void RequestSceneChange(SceneType SceneType, TransitionEffectType TransitionEffectType, Action Action)
        {
            if (IsChangingScene) return;

            IsChangingScene = true;
            //sceneHasChangedDraw = SceneHasChanged = false;
            TransitionEffect = BuildTransition(TransitionEffectType, Action);
        }

        public void RequestSceneChange(SceneType SceneType, TransitionEffectType TransitionEffectType, object NextSceneParameter = null, float transitionWaitTime = 0f)
        {
            if (IsChangingScene) return;

            IsChangingScene = true;

            elapsedTime = 0;
            this.transitionWaitTime = transitionWaitTime;

#if DEBUG
            DebugHandler.Instance.Clear();
#endif

            sceneTransitionThread = PrepareSceneChangeThread(SceneType, NextSceneParameter);
            sceneTransitionThread.Name = $"{SceneType} transition thread";

            TransitionEffect = BuildTransition(TransitionEffectType, sceneTransitionThread.Start);

            if (TransitionEffectType == TransitionEffectType.None)
                while (sceneTransitionThread.IsAlive) ;
        }

        public Thread PrepareSceneChangeThread(SceneType SceneType, object NextSceneParameter)
        {
            return new Thread(() =>
            {
                GameScene gs = BuildNextScene(SceneType, NextSceneParameter);
                gs.Initialize(GraphicsDevice, SpriteBatch);
                lock (NextScene) NextScene.Enqueue(gs);

#if DEBUG
                Console.WriteLine($"{Thread.CurrentThread.Name} has ended");
#endif
            });
        }

        public void Draw(GameTime GameTime)
        {
            if (Parameter.ShouldChangeBGScale)
            {
                CurrentScene.BeginOverscaledBackgroundDraw();
                CurrentScene.OverscaledBackgroundDraw(GameTime);
                CurrentScene.EndDraw();
            }

            CurrentScene.BeginDraw();
            CurrentScene.Draw(GameTime);

            if (TransitionEffect != null && elapsedTime > transitionWaitTime)
                TransitionEffect.Draw(GameTime, SpriteBatch);

#if DEBUG
            DebugHandler.Instance.Draw(SpriteBatch);
#endif
            CurrentScene.EndDraw();
        }

        public void Update(GameTime GameTime)
        {
            InputHandler.Update();

            CurrentScene.Update(GameTime);

            elapsedTime += (float)GameTime.ElapsedGameTime.TotalSeconds;

            if (TransitionEffect != null && elapsedTime > transitionWaitTime)
                TransitionEffect.Update(GameTime);

#if DEBUG
            DebugHandler.Instance.Update();
#endif
            ChangeScene();
        }
    }
}
