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
using Microsoft.Xna.Framework.Input;
using OpenBound.Common;
using OpenBound.GameComponents.Animation;
using OpenBound.GameComponents.Input;
using OpenBound.GameComponents.Interface;
using OpenBound.GameComponents.Interface.Popup;
using OpenBound_Network_Object_Library.Entity;
using OpenBound_Network_Object_Library.Entity.Sync;
using OpenBound_Network_Object_Library.Security;
using System;
using System.Collections.Generic;
using System.Threading;
using OpenBound_Network_Object_Library.Models;
using OpenBound.GameComponents.WeatherEffect;
using Microsoft.Xna.Framework.Graphics;
using OpenBound.GameComponents.Pawn;
using OpenBound.GameComponents.Interface.Text;
using OpenBound_Network_Object_Library.Entity.Text;
using OpenBound_Network_Object_Library.Common;
using System.Diagnostics;
using OpenBound.GameComponents.Interface.Interactive.GameList;
using System.Data.OleDb;
using OpenBound_Network_Object_Library.FileManagement;
using System.Linq;
using OpenBound.Extension;
using OpenBound_Network_Object_Library.FileManager;

namespace OpenBound.GameComponents.Level.Scene
{
    public class DebugScene : LevelScene
    {
        public float sceneTimespan;
        public bool hasRequestedNextScene;

        public PopupGameOptions optionsMenu;
        public PopupSelectMobile popupSelectMobile;

        internal static void InitializeObjects()
        {
            ConfigFileManager.CreateConfigFile(RequesterApplication.Launcher);
            ConfigFileManager.LoadConfigFile(RequesterApplication.Launcher);

            Parameter.Initialize(new LauncherInformation(
                LauncherOperationStatus.AuthConfirmed,
                ConfigFileManager.ReadClientInformation(),
                GameInformation.Instance.PlayerInformation = new Player()
                {
                    ID = 0,
                    Gender = Gender.Female,
                    Nickname = "Winged",
                    //SelectedItemTypeList,
                }));

            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.PowerUp);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.PowerUp);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.BungeShot);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.BungeShot);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.Blood);
            GameInformation.Instance.PlayerInformation.SelectedItemTypeList.Add(ItemType.Blood);

            #region Sync Mobile List
            List<Player> pList = new List<Player>()
            {
                new Player()
                {
                    Gender = Gender.Female,
                    Email = "c@c.com",
                    ID = 0,
                    Nickname = "Wicked",
                    Password = "123",
                    PlayerRank = PlayerRank.Staff4,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Ice,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Red,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" },
                    Attribute = new int[]{ 1, 1, 1, 1, 1, 1, 1, 1 },
                },
                new Player()
                {
                    Gender = Gender.Male,
                    Email = "c@c.com",
                    ID = 1,
                    Nickname = "Zicoman",
                    Password = "123",
                    PlayerRank = PlayerRank.Dragon1,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Lightning,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Blue,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Email = "c@c.com",
                    ID = 2,
                    Nickname = "Big String  To Test The Box Size Calc",
                    Password = "123",
                    PlayerRank = PlayerRank.Axe1,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Knight,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Red,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Email = "c@c.com",
                    ID = 3,
                    Nickname = "Wicko",
                    Password = "123",
                    PlayerRank = PlayerRank.GM,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Dragon,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Blue,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Email = "c@c.com",
                    ID = 4,
                    Nickname = "WickoWicko",
                    Password = "123",
                    PlayerRank = PlayerRank.DGoldenAxe2,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Dragon,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Blue,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Email = "c@c.com",
                    ID = 5,
                    Nickname = "Ordinary World",
                    Password = "123",
                    PlayerRank = PlayerRank.Chick,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Turtle,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Blue,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Email = "c@c.com",
                    ID = 6,
                    Nickname = "KonoDioDa",
                    Password = "123",
                    PlayerRank = PlayerRank.Champion1,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.Dragon,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Red,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                },
                new Player()
                {
                    Gender = Gender.Female,
                    Email = "c@c.com",
                    ID = 7,
                    Nickname = "JosephJoestar",
                    Password = "123",
                    PlayerRank = PlayerRank.Axe1,
                    PlayerRoomStatus = PlayerRoomStatus.Ready,
                    PrimaryMobile = MobileType.RaonLauncher,
                    SecondaryMobile = MobileType.Knight,
                    PlayerTeam = PlayerTeam.Red,
                    Guild = new Guild() { GuildMembers = new List<Player>(), ID = 1, Name = "Zica", Tag = "zicamasters" },
                    PlayerStatus = PlayerStatus.Normal,
                    SecurityToken = new SecurityToken() { DateTime = DateTime.Now, Token = "123123123", UnifiedSecurityToken = "123123123" }
                }
            };

            sMobList = new List<SyncMobile>();

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[0], MobileType.Lightning),
                Owner = pList[0],
                Position = new int[] { 340, 100 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[1], MobileType.Knight),
                Owner = pList[1],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[2], MobileType.Knight),
                Owner = pList[2],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[3], MobileType.Knight),
                Owner = pList[3],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[4], MobileType.Knight),
                Owner = pList[4],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[5], MobileType.Knight),
                Owner = pList[5],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[6], MobileType.Knight),
                Owner = pList[6],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });

            sMobList.Add(new SyncMobile()
            {
                CrosshairAngle = 10,
                Delay = 500,
                //Facing = Facing.Left,
                MobileMetadata = MobileMetadata.BuildMobileMetadata(pList[7], MobileType.Knight),
                Owner = pList[7],
                Position = new int[] { 1, 0 },
                SelectedShotType = ShotType.SS,
                SynchronizableActionList = new List<SynchronizableAction>() { SynchronizableAction.LeftMovement, SynchronizableAction.ChargingShot }
            });
            #endregion

            //delayboard = new Delayboard(mobList, new Vector2(0, 0) /* Parameter.ScreenCenter */);

            GameInformation.Instance.RoomMetadata = new RoomMetadata(GameMode.Score, TurnsToSuddenDeath.Turn40, MatchSuddenDeathType.DoubleDeath, RoomSize.FourVsFour, sMobList[0].Owner, 1, "123", "123")
            {
                SpawnPositions = new Dictionary<int, int[]>()
                {
                    { 0, new int[] { 1150, 700 } },
                    { 1, new int[] { 300, 700 } }, { 2, new int[] { 700, 700 } },
                    { 3, new int[] { 400, 700 } }, { 4, new int[] { 800, 700 } },
                    { 5, new int[] { 500, 700 } }, { 6, new int[] { 900, 700 } },
                    { 7, new int[] { 600, 700 } }, { 8, new int[] { 1000, 700 } },
                },
                TeamA = new List<Player>() { sMobList[0].Owner , sMobList[1].Owner, sMobList[2].Owner, sMobList[3].Owner  },
                TeamB = new List<Player>() { sMobList[4].Owner, sMobList[5].Owner, sMobList[6].Owner, sMobList[7].Owner },
            };
            GameInformation.Instance.RoomMetadata.Map = Map.GetMap(GameMapType.B, GameMap.Metamine) ;
        }


        //Delayboard delayboard;

        static List<MobileFlipbook> mFlipbook;
        static List<SyncMobile> sMobList;
        //static List<Mobile> mobList;

        public DebugScene()
        {
            sceneTimespan = 1f;
            hasRequestedNextScene = false;

            Camera.Zoom = new Vector2(1, 1);

            #region Mobile List
            /*
            mobList = new List<Mobile>();
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[0].Owner, new Vector2(100 * 1, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[1].Owner, new Vector2(100 * 2, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[2].Owner, new Vector2(100 * 3, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[3].Owner, new Vector2(100 * 4, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[4].Owner, new Vector2(100 * 5, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[5].Owner, new Vector2(100 * 6, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[6].Owner, new Vector2(100 * 7, 100)));
            mobList.Add(ActorBuilder.BuildMobile(MobileType.Knight, sMobList[7].Owner, new Vector2(100 * 8, 100)));

            for (int i = 0; i < 8; i++) ;*/

            MobileList.ForEach((x) =>
            {
                x.SyncMobile = sMobList[MobileList.IndexOf(x)];
            });
            #endregion

            MatchMetadata = new MatchMetadata();
            //MatchMetadata.CurrentTurnOwner = MobileList[0].SyncMobile;

            //optionsMenu.ShouldRender = true;

            //Popup create char
            popupSelectMobile = new PopupSelectMobile((x) => { }, () => { });
            PopupHandler.Add(popupSelectMobile);

            //MobFlipbook

            mFlipbook = new List<MobileFlipbook>();

            for (int k = 0; k < 21; k++)
                mFlipbook.Add(MobileFlipbook.CreateMobileFlipbook(MobileType.RaonLauncher, new Vector2(-500 + 100 * (k % 5), -500 + 100 * (k / 5))));

            int i = 0;

            mFlipbook[i++].ChangeState(ActorFlipbookState.Stand, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.StandLowHealth, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.Moving, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.MovingLowHealth, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.UnableToMove, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.Emotion1, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.Emotion2, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.BeingDamaged1, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.BeingDamaged2, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.BeingShocked, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.BeingFrozen, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.ChargingS1, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.ShootingS1, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.ChargingS2, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.ShootingS2, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.ChargingSS, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.ShootingSS, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.UsingItem, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.Dead, true);

            mFlipbook[i++].ChangeState(ActorFlipbookState.Falling, true);
            mFlipbook[i++].ChangeState(ActorFlipbookState.All, true);
        }

        public override void Initialize(GraphicsDevice GraphicsDevice, SpriteBatch SpriteBatch)
        {
            base.Initialize(GraphicsDevice, SpriteBatch);
        }
        
        System.Random r = new System.Random();

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (InputHandler.IsBeingPressed(Keys.F1))
            {
                //CurrentTurnOwner.LoseTurn();
                MobileList[0].GrantTurn();
                MobileList[0].LayerDepth = 0.6f;
                MobileList.ForEach((x) => x.Movement.RemainingStepsThisTurn = 100000);
            }

            if (InputHandler.IsBeingPressed(Keys.F4))
            {
                MatchMetadata.WindForce = r.Next(0, 35);
                MatchMetadata.WindAngleDegrees = r.Next(0, 360);
                HUD.WindCompass.ChangeWind(MatchMetadata.WindAngleDegrees, MatchMetadata.WindForce);
            }

            if (InputHandler.IsBeingPressed(Keys.F5))
            {
                WeatherHandler.Add(WeatherType.Mirror, new Vector2(-500, -Topography.MapHeight / 2));
                WeatherHandler.Add(WeatherType.Mirror, new Vector2(-300, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F6))
            {
                WeatherHandler.Add(WeatherType.Weakness, new Vector2(-400, -Topography.MapHeight / 2));
                WeatherHandler.Add(WeatherType.Weakness, new Vector2(0, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F7))
            {
                WeatherHandler.Add(WeatherType.Force, new Vector2(-300, -Topography.MapHeight / 2));
                WeatherHandler.Add(WeatherType.Force, new Vector2(0, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F8))
            {
                WeatherHandler.Add(WeatherType.Tornado, new Vector2(400, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F9))
            {
                WeatherHandler.Add(WeatherType.Electricity, new Vector2(-100, -Topography.MapHeight / 2));
                WeatherHandler.Add(WeatherType.Electricity, new Vector2(0, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F10))
            {
                WeatherHandler.Add(WeatherType.Random, WeatherType.Weakness, new Vector2(0, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F11))
            {
                Color green = new Color(57, 255, 20, 255 / 4);
                Color blue = new Color(27, 3, 163, 255 / 4);
                Color red = new Color(255, 7, 58, 255 / 4);
                Color white = new Color(255, 255, 255, 255 / 4);
                Color yellow = new Color(204, 255, 0, 255 / 4);

                Vector2 position = Vector2.Zero;

                for (int i = 0; i < 10; i++)
                {
                    SpecialEffectBuilder.ThorShot(position, Color.Lerp(green, blue, i / 9f), 6, 0);
                    position += Vector2.UnitX * 15;
                }

                for (int i = 0; i < 10; i++)
                {
                    SpecialEffectBuilder.ThorShot(position, Color.Lerp(blue, yellow, i / 9f), 6, 0);
                    position += Vector2.UnitX * 15;
                }

                for (int i = 0; i < 10; i++)
                {
                    SpecialEffectBuilder.ThorShot(position, Color.Lerp(yellow, red, i / 9f), 6, 0);
                    position += Vector2.UnitX * 15;
                }

                for (int i = 0; i < 10; i++)
                {
                    SpecialEffectBuilder.ThorShot(position, Color.Lerp(red, white, i / 9f), 6, 0);
                    position += Vector2.UnitX * 15;
                }
            }

            if (InputHandler.IsBeingPressed(Keys.F12))
            {
                MineList[0].GrantTurn();
            }

            if (InputHandler.IsBeingPressed(Keys.F2))
            {
                WeatherHandler.Add(WeatherType.Thor, WeatherType.Weakness, new Vector2(0, -Topography.MapHeight / 2));
            }

            if (InputHandler.IsBeingPressed(Keys.F3))
            {
                SpecialEffectBuilder.RaonLauncherProjectile2ActiveTornado(MobileList[0].Position);
            }

            if (InputHandler.IsBeingPressed(Keys.D1))
            {
                SpecialEffectBuilder.RaonLauncherProjectile2DormantTornado(MobileList[0].Position);
            }

            if (InputHandler.IsBeingPressed(Keys.D2))
            {
                MobileList[3].Die();
            }

            if (InputHandler.IsBeingPressed(Keys.D3))
            {
                MobileList[0].ChangeFlipbookState(ActorFlipbookState.BeingDamaged2, true);
            }


            if (InputHandler.IsBeingPressed(Keys.D4))
            {
                new PopupSelectItem()
                {
                    ShouldRender = true
                };
            }

            if (InputHandler.IsBeingPressed(Keys.D5))
            {
                HUD.OnReceiveMessageAsyncCallback(new PlayerMessage()
                {
                    Player = MobileList[0].Owner,
                    PlayerTeam = PlayerTeam.Red,
                    Text = "Abiruindjo abirual"
                }, 0);

                HUD.OnReceiveMessageAsyncCallback(new CustomMessage()
                {
                    FontTextType = FontTextType.Consolas10,
                    TextBorderColor = Color.Red.PackedValue,
                    TextColor = Color.Blue.PackedValue,
                    Text = "Abiruindjo abirual"
                }, 1);
            }

            if (InputHandler.IsBeingPressed(Keys.D6))
            {
                PopupHandler.Add(new PopupSelectItem() { ShouldRender = true });
            }

            if (InputHandler.IsBeingPressed(Keys.D7))
            {
                SceneHandler.Instance.RequestSceneChange(SceneType.DebugScene, TransitionEffectType.RotatingRectangles);
            }

            if (InputHandler.IsBeingPressed(Keys.D8))
            {
                //SpecialEffectBuilder.HitEffect(new Vector2(100, 100), 4, MathHelper.TwoPi * (float)Parameter.Random.NextDouble());
                for (int i = 0; i < 8; i++) MobileList[0].Owner.Attribute[i] = 1;
            }

            if (InputHandler.IsBeingPressed(Keys.D9))
            {
                //MobileList[0].ReceiveDamage(10);
                for (int i = 0; i < 8; i++)
                {
                    if (Parameter.Random.NextDouble() < 0.1)
                        MobileList[0].Owner.Attribute[i] =
                            Math.Sign(MobileList[0].Owner.Attribute[i]) *
                                (Math.Abs(MobileList[0].Owner.Attribute[i]) + 1) * -1;
                }

                //HUD.StatusBarDictionary.ForEach((x) => x.UpdateAttributeList());
            }

            if (InputHandler.IsBeingReleased(Keys.D0))
            {
                //tb = new TextBalloon(MobileList[0], "123123123 123123123 1231231234814812749817498174 71829741984712984721941928471924721984");
            }

            tb?.Update(gameTime);
        }

        TextBalloon tb;

        ParticleEmitter pe = new ParticleEmitter(
            () =>
            {
                SpecialEffectBuilder.RandomLowHealthSmokeParticle(MobileList[0].Position);
            },
            0.1f);

        public override void Draw(GameTime gameTime)
        {
            mFlipbook.ForEach((x) => x.Draw(gameTime, spriteBatch));
            tb?.Draw(spriteBatch);
            base.Draw(gameTime);

            //optionsMenu.Draw(GameTime, spriteBatch);
            //delayboard.Draw(GameTime, SpriteBatch);
            //mobList.ForEach((x) => x.Draw(GameTime, SpriteBatch));
        }
    }
}
