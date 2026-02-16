using System.Collections;

using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.State;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Core
{
    public class Recorder
    {
        public Recorder()
        {
            Instance = this;

            Events.OnPreRecord += OnPreRecord;
            Events.OnStartRecording += OnPostRecord;
            Events.OnRecordTick += OnRecordTick;
            Events.OnStopRecording += OnStopRecording;
        }

        public static Recorder Instance { get; private set; }

        public float RecordingTime { get => m_recordingTime; }
        public float TakeTime;

        public int RecordTick { get => m_recordTick; }

        public int Countdown { get; private set; }

        public List<Actor> ActiveActors = new List<Actor>();

        public Actor ActiveActor { get => m_activeActor; }
        public Actor LastActor { get => m_lastActor; }

        private Actor m_activeActor;
        private Actor m_lastActor;

        private Coroutine m_recordRoute;

        private float m_fpsTimer = 0f;

        private float m_recordingTime;

        private float m_timeSinceLastTick = 0;

        private int m_recordTick;

        public void SetActor(MarrowAvatar avatar)
        {
            m_lastActor = m_activeActor;
            m_activeActor = new Actor(avatar);
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
            if (m_recordRoute == null)
            {
                m_recordRoute = MelonCoroutines.Start(RecordRoutine()) as Coroutine;
            }
        }

        public void RecordCamera()
        {
            foreach (var castMember in Director.Cast)
            {
                castMember?.Act();
            }
        }

        public void RecordActor()
        {
            if (Settings.World.recordActors)
            {
                m_activeActor.RecordFrame();
            }
            
            foreach (var prop in Director.RecordingProps)
            {
                prop.Record(m_recordTick);
            }

            foreach (var castMember in Director.Cast)
            {
                Playback.Instance.AnimateActor(castMember);
            }

            foreach(var prop in Director.WorldProps)
            {
                Playback.Instance.AnimateProp(prop);
            }
        }

        /// <summary>
        /// Called when we first hit the record button.
        /// </summary>
        public void OnPreRecord()
        {
            if (m_recordTick > 0)
            {
                m_recordTick = 0;
            }

            Playback.Instance.ResetPlayhead();

            m_fpsTimer = 0f;

            m_recordingTime = 0f;

            if (Settings.World.recordActors)
            {
                SetActor(Constants.RigManager.avatar);
            }

            foreach (var castMember in Director.Cast)
            {
                castMember.OnSceneBegin();
            }

            foreach(var prop in Director.WorldProps)
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
            m_activeActor?.Microphone?.SetCorrectionMode(Audio.ActorSpeech.AudioCorrectionMode.Corrected);
            m_activeActor?.Microphone?.RecordMicrophone();

            foreach (Trackable castMember in Director.Cast)
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

            m_recordTick++;
            m_recordingTime += m_timeSinceLastTick;

            // keep up!
            if (m_recordingTime > TakeTime)
            {
                TakeTime = m_recordingTime;
            }

            Playback.Instance.MovePlayhead(m_timeSinceLastTick);

            if (Director.CaptureState == CaptureState.CaptureCamera)
            {
                RecordCamera();
            }
            
            if (Director.CaptureState == CaptureState.CaptureActor)
            {
                RecordActor();
            }

            foreach (var castMember in Director.Cast)
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
            m_activeActor?.Microphone?.StopRecording();

            foreach (Trackable castMember in Director.Cast)
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

            if (Settings.World.recordActors)
            {
                m_activeActor.CloneAvatar();
                
                foreach (var recordedProp in Director.RecordingProps)
                {
                    m_activeActor.OwnProp(recordedProp);
                }

                Director.Cast.Add(m_activeActor);
            }
            
            m_lastActor = m_activeActor;

            m_activeActor = null;

            Director.Cast.AddRange(ActiveActors);
            ActiveActors.Clear();

            Director.WorldProps.AddRange(Director.RecordingProps);
            Director.LastRecordedProps = Director.RecordingProps;
            Director.RecordingProps.Clear();

            if (m_recordRoute != null)
            {
                MelonCoroutines.Stop(m_recordRoute);
                m_recordRoute = null;
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

            FeedbackSFX.BeepHigh();

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
                    m_timeSinceLastTick += Time.deltaTime;
                else
                    m_timeSinceLastTick += Time.unscaledDeltaTime;
                
                // Temporal scaling increases the resolution when changing timescale
                if (Settings.World.temporalScaling) 
                    m_fpsTimer += Time.unscaledDeltaTime;
                else
                    m_fpsTimer += Time.deltaTime;

                if (m_fpsTimer > perTick)
                {
                    Tick();
                    m_fpsTimer = 0f;
                    m_timeSinceLastTick = 0;
                }

                yield return null;
            }
            
            Events.OnStopRecording?.Invoke();
            yield return null;
        }
    }
}
