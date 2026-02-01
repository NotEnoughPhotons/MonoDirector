using System;

using NEP.MonoDirector.State;

using MelonLoader;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class DirectedComponent(IntPtr ptr) : MonoBehaviour(ptr)
    {
        protected virtual void OnEnable()
        {
            Events.OnPlay += OnStartPlayback;
            Events.OnStopPlayback += OnStopPlayback;
            Events.OnStartRecording += OnStartRecording;
            Events.OnStopRecording += OnStopRecording;
            Events.OnPlayStateSet += OnPlayStateSet;
        }

        protected virtual void OnDisable()
        {
            Events.OnPlay -= OnStartPlayback;
            Events.OnStopPlayback -= OnStopPlayback;
            Events.OnStartRecording -= OnStartRecording;
            Events.OnStopRecording -= OnStopRecording;
            Events.OnPlayStateSet -= OnPlayStateSet;
        }

        protected virtual void OnStartPlayback() { }

        protected virtual void OnStopPlayback() { }

        protected virtual void OnStartRecording() { }

        protected virtual void OnStopRecording() { }

        protected virtual void OnPlayStateSet(PlayState playState) { }
    }
}
