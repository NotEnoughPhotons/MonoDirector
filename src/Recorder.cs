using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.State;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            instance = this;

            Events.OnPreRecord += OnPreRecord;
            Events.OnStartRecording += OnPostRecord;
            Events.OnRecordTick += OnRecordTick;
            Events.OnStopRecording += OnStopRecording;
        }

        public static Recorder instance;

        public float RecordingTime { get => recordingTime; }
        public float TakeTime;

        public int RecordTick { get => recordTick; }

        public int Countdown { get; private set; }

        public List<Actor> ActiveActors = new List<Actor>();

        public Actor ActiveActor { get => activeActor; }
        public Actor LastActor { get => lastActor; }

        private Actor activeActor;
        private Actor lastActor;

        private Coroutine recordRoutine;

        private float fpsTimer = 0f;

        private float recordingTime;

        private float timeSinceLastTick = 0;

        private int recordTick;

        public void SetActor(MarrowAvatar avatar)
        {
            lastActor = activeActor;
            activeActor = new Actor(avatar);
        }

        public void Tick()
        {
            if (Director.PlayState != PlayState.Recording)
            {
                return;
            }

            Events.OnRecordTick?.Invoke();
        }

        public void StartRecordRoutine()
        {
            if (recordRoutine == null)
            {
                recordRoutine = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
            }
        }

        public void RecordCamera()
        {
            foreach (var castMember in Director.instance.Cast)
            {
                castMember?.Act();
            }
        }

        public void RecordActor()
        {
            activeActor.RecordFrame();

            foreach (var prop in Director.instance.RecordingProps)
            {
                prop.Record(recordTick);
            }

            foreach (var castMember in Director.instance.Cast)
            {
                Playback.Instance.AnimateActor(castMember);
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                Playback.Instance.AnimateProp(prop);
            }
        }

        /// <summary>
        /// Called when we first hit the record button.
        /// </summary>
        public void OnPreRecord()
        {
            if (recordTick > 0)
            {
                recordTick = 0;
            }

            Playback.Instance.ResetPlayhead();

            fpsTimer = 0f;

            recordingTime = 0f;

            SetActor(Constants.RigManager.avatar);

            foreach (var castMember in Director.instance.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach(var prop in Director.instance.WorldProps)
            {
                prop.OnSceneBegin();
                prop.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Called the moment the recording begins.
        /// </summary>
        public void OnPostRecord()
        {
            activeActor?.Microphone?.RecordMicrophone();

            foreach (Trackable castMember in Director.instance.Cast)
            {
                if (castMember != null && castMember is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.Playback();
                }
            }
        }

        /// <summary>
        /// Called every time a frame is recorded
        /// </summary>
        public void OnRecordTick()
        {
            if (Director.PlayState == PlayState.Paused)
            {
                return;
            }

            recordTick++;
            recordingTime += timeSinceLastTick;

            // keep up!
            if (recordingTime > TakeTime)
            {
                TakeTime = recordingTime;
            }

            Playback.Instance.MovePlayhead(timeSinceLastTick);

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }
            
            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            foreach (var castMember in Director.instance.Cast)
            {
                if (castMember != null)
                {
                    castMember.Act();
                }
            }
        }

        /// <summary>
        /// Called when the recording stops.
        /// </summary>
        public void OnStopRecording()
        {
            activeActor?.Microphone?.StopRecording();

            foreach (Trackable castMember in Director.instance.Cast)
            {
                if (castMember != null && castMember is Actor actorPlayer)
                {
                    actorPlayer?.Microphone?.StopPlayback();
                }
            }

#if DEBUG
            /*
            Stopwatch sw = new Stopwatch();
            sw.Start();
            byte[] actorBytes = ActiveActor.ToBinary();
            sw.Stop();
            
            Main.Logger.Msg($"[STOPWATCH]: Actor::ToBinary() took {sw.ElapsedMilliseconds}...");
            
            sw.Restart();
            
            using (FileStream file = File.Open("test.mdat", FileMode.Create))
            {
                uint ident = ActiveActor.GetBinaryID();
                file.Write(BitConverter.GetBytes(ident), 0, sizeof(uint));
                
                file.Write(actorBytes, 0, actorBytes.Length);
            };
            
            sw.Stop();
            
            Main.Logger.Msg($"[STOPWATCH]: Writing MDAT took {sw.ElapsedMilliseconds}...");
            sw.Restart();
            
            // Then try to read it back
            using (FileStream file = File.Open("test.mdat", FileMode.Open))
            {
                // Seek past the first 4 bytes
                file.Seek(4, SeekOrigin.Begin);
                ActiveActor.FromBinary(file);
            }

            sw.Stop();
            
            Main.Logger.Msg($"[STOPWATCH]: Actor::FromBinary() took {sw.ElapsedMilliseconds}...");
            */
#endif
            
            activeActor.CloneAvatar();
            Director.instance.Cast.Add(activeActor);
            lastActor = activeActor;

            activeActor = null;

            Director.instance.Cast.AddRange(ActiveActors);
            ActiveActors.Clear();

            Director.instance.WorldProps.AddRange(Director.instance.RecordingProps);
            Director.instance.LastRecordedProps = Director.instance.RecordingProps;
            Director.instance.RecordingProps.Clear();

            if (recordRoutine != null)
            {
                MelonCoroutines.Stop(recordRoutine);
                recordRoutine = null;
            }
        }

        public IEnumerator RecordRoutine()
        {
            Events.OnPreRecord?.Invoke();

            for (Countdown = 0; Countdown < Settings.World.delay; Countdown++)
            {
                Events.OnTimerCountdown?.Invoke();
                yield return new WaitForSeconds(1);
            }

            Main.feedbackSFX.BeepHigh();

            Events.OnStartRecording?.Invoke();
            
            float perTick = 1.0F / Settings.World.fps;
            
            while (Director.PlayState == PlayState.Recording || Director.PlayState == PlayState.Paused)
            {
                // These are different for a reason!
                // virtual FPS and real FPS are decoupled here when asked!
                // Recording 15FPS with normal delta time means 15FPS becomes 5FPS in real frames!
                
                // Ignoring slomo means using deltaTime to store our recorded time
                // Therefore data is scaled with timescale
                if (Settings.World.ignoreSlomo) 
                    timeSinceLastTick += Time.deltaTime;
                else
                    timeSinceLastTick += Time.unscaledDeltaTime;
                
                // Temporal scaling increases the resolution when changing timescale
                if (Settings.World.temporalScaling) 
                    fpsTimer += Time.unscaledDeltaTime;
                else
                    fpsTimer += Time.deltaTime;

                if (fpsTimer > perTick)
                {
                    Tick();
                    fpsTimer = 0f;
                    timeSinceLastTick = 0;
                }

                yield return null;
            }
            
            Events.OnStopRecording?.Invoke();
            yield return null;
        }
    }
}
