using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public static class Director
    {
        public static Playback Playback { get => m_playback; }
        public static Recorder Recorder { get => m_recorder; }

        public static Film ActiveFilm { get => m_activeFilm; }
        public static Stage ActiveStage { get => m_activeStage; }

        public static Actor SelectedActor { get => m_selectedActor; }

        public static FreeCamera Camera { get => m_camera; }
        public static CameraVolume Volume { get => m_camera.GetComponent<CameraVolume>(); }

        public static PlayState PlayState { get => m_playState; }
        public static PlayState LastPlayState { get => m_lastPlayState; }
        public static CaptureState CaptureState { get => m_captureState; }

        public static int WorldTick { get => m_worldTick; }

        public static event Action<Stage> OnStageAdded;
        public static event Action<Stage> OnStageRemoved;
        public static event Action<Stage> OnStageSet;

        public static event Action<Actor> OnActorSelected;
        public static event Action<Actor> OnActorDeselected;

        private static Actor m_selectedActor;

        private static Playback m_playback;
        private static Recorder m_recorder;

        private static Film m_activeFilm;
        private static Stage m_activeStage;

        private static PlayState m_playState = PlayState.Stopped;
        private static PlayState m_lastPlayState;
        private static CaptureState m_captureState = CaptureState.CaptureActor;

        private static FreeCamera m_camera;

        private static int m_worldTick;

        internal static void Initialize()
        {
            m_playback = new Playback();
            m_recorder = new Recorder();
            Caster.Initialize();

            Caster.OnActorRecasted += (_) => Record();

            Events.OnPrePlayback += () => SetPlayState(PlayState.Preplaying);
            Events.OnPreRecord += () => SetPlayState(PlayState.Prerecording);

            Events.OnPlay += () => SetPlayState(PlayState.Playing);
            Events.OnStartRecording += () => SetPlayState(PlayState.Recording);

            m_activeFilm = new Film();
            m_activeStage = new Stage();
            m_activeFilm.AddStage(m_activeStage);
        }

        internal static void Shutdown()
        {
            Caster.OnActorRecasted -= (_) => Record();

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
            Caster.RecastActor(actor);
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

        public static void AddStage(Stage stage)
        {
            m_activeFilm.AddStage(stage);
            OnStageAdded?.Invoke(stage);
        }

        public static void RemoveStage(Stage stage)
        {
            // Removing the same stage
            if (m_activeStage.StageIndex == stage.StageIndex && m_activeFilm.Stages.Count > 1)
            {
                if (m_activeStage.StageIndex > 0)
                    m_activeStage = m_activeFilm.Stages[m_activeStage.StageIndex - 1];
                else if (m_activeStage.StageIndex == 0)
                    m_activeStage = m_activeFilm.Stages[m_activeStage.StageIndex + 1];

                SetStage(m_activeStage);
            }

            m_activeFilm.RemoveStage(stage);

            // If the film is empty, add a new stage and set it.
            if (m_activeFilm.Empty)
            {
                Stage newStage = new Stage();
                AddStage(newStage);
                SetStage(newStage);
            }

            OnStageRemoved?.Invoke(stage);
        }

        public static void SetStage(Stage stage)
        {
            if (stage == null)
            {
                Logging.WarnDebug("Director.SetStage was called with a null stage!");
                return;
            }

            foreach (var actor in Caster.Cast)
            {
                actor.ActorBody.AllowCollisions(false);
                actor.ClonedAvatar.gameObject.SetActive(false);
            }

            foreach (var prop in Caster.Props)
            {
                prop.gameObject.SetActive(false);
            }

            Caster.ClearCast();
            Caster.ClearProps();
            Caster.ClearRecordProps();

            m_activeStage = stage;

            Caster.CastActors(stage.Actors.ToList());
            Caster.AddProps(stage.Props.ToList());

            foreach (var actor in Caster.Cast)
            {
                actor.ActorBody.AllowCollisions(true);
                actor.ClonedAvatar.gameObject.SetActive(true);
                actor.OnSceneBegin();
            }

            foreach (var prop in Caster.Props)
            {
                prop.gameObject.SetActive(true);
                prop.OnSceneBegin();
            }

            OnStageSet?.Invoke(stage);
        }

        public static void SelectActor(Actor actor) => Caster.SelectActor(actor);

        public static void DeselectActor(Actor actor) => Caster.DeselectActor(actor);

        public static void RemoveActor(Actor actor) => Caster.UncastActor(actor);

        public static void RemoveAllActors()
        {
            m_playState = PlayState.Stopped;

            for (int i = Caster.Cast.Count; i > 0; i--)
            {
                Caster.UncastActor(Caster.Cast[i]);
            }

            Caster.ClearCast();
        }

        public static void ClearScene()
        {
            RemoveAllActors();
            
            for (int i = Caster.Props.Count - 1; i > 0; i--)
            {
                Caster.RemoveProp(Caster.Props[i]);
                GameObject.Destroy(Caster.Props[i]);
            }

            Caster.ClearProps();
        }

        public static void SetPlayState(PlayState state)
        {
            m_lastPlayState = m_playState;
            m_playState = state;
            Events.OnPlayStateSet?.Invoke(state);
        }
    }
}
   