using UnityEngine;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.State.Recording
{
    public sealed class RecordingState : PlayheadState
    {
        public float RecordingTime => _recordingTime;

        private int _recordFrame;
        private float _perTickUpdate;
        private float _timeSinceLastTick;
        private float _fpsTimer;
        private float _recordingTime;

        public override void Start()
        {
            _recordFrame = 0;
            _timeSinceLastTick = 0f;
            _fpsTimer = 0f;
            _recordingTime = 0f;
            _perTickUpdate = 1f / Settings.World.FPS;

            Actor activeActor = Director.Instance.ActiveActor;

            foreach (Actor castMember in Director.Instance.Cast)
            {
                
            }

            Director.Instance.DetermineMode(this);
        }

        public override void Process()
        {
            if (Settings.World.IgnoreSlowmo)
            {
                _timeSinceLastTick += Time.deltaTime;
            }
            else
            {
                _timeSinceLastTick += Time.unscaledDeltaTime;
            }

            if (Settings.World.TemporalScaling)
            {
                _fpsTimer += Time.unscaledDeltaTime;
            }
            else
            {
                _fpsTimer += Time.deltaTime;
            }

            if (_fpsTimer > _perTickUpdate)
            {
                ProcessFrame();
                _fpsTimer = 0f;
                _timeSinceLastTick = 0f;
            }
        }

        public override void Stop()
        {
            FeedbackSFX.Instance.Beep();

            Actor activeActor = Director.Instance.ActiveActor;
            activeActor.CloneAvatar();

            Director.Instance.CastActor(activeActor);
            Director.Instance.TransferRecordingProps();
        }

        private void ProcessFrame()
        {
            _recordFrame++;
            _recordingTime += _timeSinceLastTick;

            if (_recordingTime > Director.Instance.Playhead.TakeTime)
            {
                Director.Instance.Playhead.SetTakeTime(_recordingTime);
            }

            Director.Instance.Playhead.SetRecordingTime(this);

            Director.Instance.Playhead.Move(_timeSinceLastTick);

            if (Director.Instance.CaptureType == CaptureType.CaptureCamera)
            {
                // RecordCamera();
            }

            if (Director.Instance.CaptureType == CaptureType.CaptureActor)
            {
                RecordActor();
            }

            Director.Instance.AnimateAll();
        }

        private void RecordActor()
        {
            Director.Instance.ActiveActor.RecordFrame();

            foreach (Prop prop in Director.Instance.RecordingProps)
            {
                prop.Record(_recordFrame);
            }
        }
    }
}
