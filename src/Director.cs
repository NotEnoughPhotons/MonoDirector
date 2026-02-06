using UnityEngine;

using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;

using System.Collections.Generic;
using NEP.MonoDirector.Actors;
using BoneLib;

namespace NEP.MonoDirector.Core
{
    public static class Director
    {
        public static Playback Playback { get => m_playback; }
        public static Recorder Recorder { get => m_recorder; }

        public static FreeCamera Camera { get => m_camera; }
        public static CameraVolume Volume { get => m_camera.GetComponent<CameraVolume>(); }

        public static PlayState PlayState { get => m_playState; }
        public static PlayState LastPlayState { get => m_lastPlayState; }
        public static CaptureState CaptureState { get => m_captureState; }

        public static List<Actor> Cast;
        public static List<ActorNPC> NPCCast;

        public static List<Prop> WorldProps;
        public static List<Prop> RecordingProps;
        public static List<Prop> LastRecordedProps;

        public static int WorldTick { get => m_worldTick; }

        private static Playback m_playback;
        private static Recorder m_recorder;

        private static PlayState m_playState = PlayState.Stopped;
        private static PlayState m_lastPlayState;
        private static CaptureState m_captureState = CaptureState.CaptureActor;

        private static FreeCamera m_camera;

        private static int m_worldTick;

        internal static void Initialize()
        {
            m_playback = new Playback();
            m_recorder = new Recorder();

            Cast = new List<Actor>();
            NPCCast = new List<ActorNPC>();
            WorldProps = new List<Prop>();
            RecordingProps = new List<Prop>();

            Events.OnPrePlayback += () => SetPlayState(PlayState.Preplaying);
            Events.OnPreRecord += () => SetPlayState(PlayState.Prerecording);

            Events.OnPlay += () => SetPlayState(PlayState.Playing);
            Events.OnStartRecording += () => SetPlayState(PlayState.Recording);
        }

        internal static void Shutdown()
        {
            Events.OnPrePlayback -= () => SetPlayState(PlayState.Preplaying);
            Events.OnPreRecord -= () => SetPlayState(PlayState.Prerecording);

            Events.OnPlay -= () => SetPlayState(PlayState.Playing);
            Events.OnStartRecording -= () => SetPlayState(PlayState.Recording);
        }

        private static void Update()
        {
            if (!Settings.Debug.useKeys)
            {
                return;
            }

            float seekRate = Playback.Instance.PlaybackRate * Time.deltaTime;
            
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                Playback.Instance.Seek(-seekRate);
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                Playback.Instance.Seek(seekRate);
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                Play();
            }

            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                Record();
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                Stop();
            }
        }

        public static void Play()
        {
            Playback.BeginPlayback();
        }

        public static void Pause()
        {
            SetPlayState(PlayState.Paused);
        }

        public static void Record()
        {
            Recorder.StartRecordRoutine();
        }

        public static void Recast(Actor actor)
        {
            Vector3 actorPosition = actor.Frames[0].TransformFrames[0].position;
            Constants.RigManager.Teleport(actorPosition, true);
            Constants.RigManager.SwapAvatar(actor.ClonedAvatar);

            // Any props recorded by this actor must be removed if we're recasting
            // If we don't, the props will still play, but they will be floating in the air aimlessly.
            // Spooky!

            if (WorldProps.Count != 0)
            {
                foreach (var prop in WorldProps)
                {
                    if (prop.Actor == actor)
                    {
                        GameObject.Destroy(prop);
                    }
                }
            }

            Cast.Remove(actor);
            actor.Delete();

            Record();
        }

        public static void Stop()
        {
            SetPlayState(PlayState.Stopped);
        }

        public static void SetCamera(FreeCamera camera)
        {
            m_camera = camera;
        }

        public static void RemoveActor(Actor actor)
        {
            Cast.Remove(actor);
            actor.Delete();
        }

        public static void RemoveLastActor()
        {
            RemoveActor(Recorder.Instance.LastActor);

            foreach(var prop in LastRecordedProps)
            {
                WorldProps.Remove(prop);
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }
        }

        public static void RemoveAllActors()
        {
            m_playState = PlayState.Stopped;

            for (int i = 0; i < Cast.Count; i++)
            {
                Cast[i].Delete();
            }

            Cast.Clear();
        }
        
        public static void ClearLastProps()
        {
            foreach(var prop in LastRecordedProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                WorldProps.Remove(prop);
                GameObject.Destroy(prop);
            }

            LastRecordedProps.Clear();
        }

        public static void ClearScene()
        {
            RemoveAllActors();
            
            foreach(var prop in WorldProps)
            {
                prop.InteractableRigidbody.isKinematic = false;
                GameObject.Destroy(prop);
            }

            WorldProps.Clear();
        }

        public static void SetPlayState(PlayState state)
        {
            m_lastPlayState = m_playState;
            m_playState = state;
            Events.OnPlayStateSet?.Invoke(state);
        }
    }
}
   