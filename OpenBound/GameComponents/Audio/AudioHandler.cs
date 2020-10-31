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

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using OpenBound.Common;
using OpenBound.GameComponents.Asset;
using OpenBound_Network_Object_Library.FileOutput;
using System;
using System.Collections.Generic;
using System.Threading;

namespace OpenBound.GameComponents.Audio
{
    public enum ChangeEffect
    {
        Fade,
        None,
    }

    class AudioHandler
    {
        public static float MasterVolume = 1f;
        public static float BGMVolume;
        public static float SFXVolume;

        private static Thread songFadeoutThread;
        private static Queue<List<string>> songFadeoutQueue;
        private static List<string> currentActiveSonglist;

        //PlayUniqueSE
        private static HashSet<SoundEffect> playingSoundEffects;

        public static void ChangeBGMVolume(int newVolume)
        {
            BGMVolume = newVolume / 100f;

            MediaPlayer.Volume = MasterVolume * BGMVolume;
        }

        public static void ChangeSFXVolume(int newVolume)
        {
            SFXVolume = newVolume / 100f;
        }

        public static void Initialize()
        {
            GameClientSettingsInformation gcsi = Parameter.GameClientSettingsInformation;
#if DEBUGSCENE
            ChangeSFXVolume(20);
            ChangeBGMVolume(0);
#else
            ChangeSFXVolume(gcsi.SFX);
            ChangeBGMVolume(gcsi.BGM);
#endif
            //MediaPlayer.Volume = MasterVolume * FXVolume;
            //MediaPlayer.IsRepeating = true;

            currentActiveSonglist = new List<string>();
            songFadeoutQueue = new Queue<List<string>>();
            playingSoundEffects = new HashSet<SoundEffect>();
        }

        public static void PlaySoundEffect(string path, float volume = 1f, float pitch = 0f, float pan = 0f)
        {
            AssetHandler.Instance.RequestSoundEffect(path).Play(MasterVolume * SFXVolume * volume, pitch, pan);
        }

        /// <summary>
        /// Ensures that the sound being played is unique, not allowing it to be played again or multiple times until it is fully reproduced. Or aborted via AbortPlayingUniqueSound.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="volume"></param>
        /// <param name="pitch"></param>
        /// <param name="pan"></param>
        public static void PlayUniqueSoundEffect(SoundEffect soundEffect, Func<bool> existanceCondition, float volume = 1f, float pitch = 0f, float pan = 0f, int delay = 0)
        {
            lock (playingSoundEffects)
            {
                if (!playingSoundEffects.Contains(soundEffect))
                {
                    new Thread(() =>
                    {
                        Thread.Sleep(delay);
                        if (!existanceCondition()) return;

                        lock (playingSoundEffects)
                            playingSoundEffects.Add(soundEffect);

                        SoundEffectInstance sei = soundEffect.CreateInstance();
                        sei.Pitch = pitch;
                        sei.Volume = MasterVolume * SFXVolume * volume;
                        sei.Pan = pan;
                        sei.IsLooped = true;

                        sei.Play();

                        float time = 0;
                        const int timeCheck = 40;
                        
                        while (existanceCondition())
                        {
                            time += timeCheck;
                            Thread.Sleep(timeCheck);

                            if (time > soundEffect.Duration.TotalMilliseconds)
                                time = 0;
                        }

                        lock (playingSoundEffects)
                            playingSoundEffects.Remove(soundEffect);

                        sei.Stop();
                    }).Start();
                }
            }
        }

        public static void PlaySoundEffectWhile(SoundEffect soundEffect, Func<bool> existanceCondition, float volume = 1f, float pitch = 0f, float pan = 0f, int delay = 0)
        {
            new Thread(() =>
            {
                Thread.Sleep(delay);
                if (!existanceCondition()) return;

                lock (playingSoundEffects)
                    playingSoundEffects.Add(soundEffect);

                SoundEffectInstance sei = soundEffect.CreateInstance();
                sei.Pitch = pitch;
                sei.Volume = MasterVolume * SFXVolume * volume;
                sei.Pan = pan;
                sei.IsLooped = true;

                sei.Play();

                float time = 0;
                const int timeCheck = 40;

                while (existanceCondition())
                {
                    time += timeCheck;
                    Thread.Sleep(timeCheck);

                    if (time > soundEffect.Duration.TotalMilliseconds)
                        time = 0;
                }

                sei.Stop();
            }).Start();
        }

        private static void PlayRandomSong(List<string> trackList)
        {

            Song s = AssetHandler.Instance.RequestSong(trackList[Parameter.Random.Next(0, trackList.Count)]);
            MediaPlayer.Play(s);
            MediaPlayer.IsRepeating = true;
        }

        public static void ChangeSong(List<string> trackList, ChangeEffect changeEffect = ChangeEffect.None)
        {
            lock (currentActiveSonglist)
                if (currentActiveSonglist == trackList)
                    return;

            switch (changeEffect)
            {
                case ChangeEffect.None:
                    lock (currentActiveSonglist)
                        currentActiveSonglist = trackList;

                    PlayRandomSong(trackList);
                    break;
                case ChangeEffect.Fade:
                    lock (songFadeoutQueue)
                        songFadeoutQueue.Enqueue(trackList);

                    if (songFadeoutThread == null || !songFadeoutThread.IsAlive)
                    {
                        songFadeoutThread = new Thread(ChangeSongFadeThread);
                        songFadeoutThread.Name = "SongChangeEffectFadeThread";
                        songFadeoutThread.Start();
                    }

                    break;
            }
        }

        private static void ChangeSongFadeThread()
        {
            float volumeFactor = 1f;
            int queueCount = 0;

            do
            {
                while (volumeFactor > 0)
                {
                    Thread.Sleep(50);
                    volumeFactor -= 0.1f;
                    MediaPlayer.Volume = MasterVolume * BGMVolume * volumeFactor;
                }

                lock (songFadeoutQueue)
                {
                    lock (currentActiveSonglist)
                        currentActiveSonglist = songFadeoutQueue.Dequeue();

                    PlayRandomSong(currentActiveSonglist);
                    queueCount = songFadeoutQueue.Count;
                }

                while (volumeFactor < 1f)
                {
                    Thread.Sleep(50);
                    volumeFactor += 0.1f;
                    MediaPlayer.Volume = MasterVolume * BGMVolume * volumeFactor;
                }


            } while (queueCount > 0);
        }
    }
}
