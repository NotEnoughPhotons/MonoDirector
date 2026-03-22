using System;
using System.Collections;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public class Playback
    {
        public Playback()
        {
            Instance = this;

            Events.OnPrePlayback += OnPrePlayback;
            Events.OnPlay += OnPlay;
            Events.OnPlaybackTick += OnPlaybackTick;
            Events.OnStopPlayback += OnStopPlayback;
        }

        public static Playback Instance { get; private set; }
        
        /// <summary>
        /// The current time stamp of the playhead
        /// </summary>
        public float PlaybackTime => m_playbackTime;
        
        /// <summary>
        /// The rate at which the playhead seeks, similar to Time.timeScale
        /// </summary>
        public float PlaybackRate = 1;
        // TODO: Should this be private for some reason?

        public int Countdown { get; private set; }

        private float m_playbackTime;
        private Coroutine m_playRoutine;

        private Film m_film;
        private Stage m_stage;

        //
        // Playback modification methods
        //
        public void ResetPlayhead() => m_playbackTime = 0f;

        public void MovePlayhead(float amount) => m_playbackTime += amount;

        //
        // Playback methods
        //
        
        /// <summary>
        /// Called per frame, invokes any and all OnPlaybackTick delegates
        /// </summary>
        public void Tick()
        {
            if (Director.PlayState != PlayState.Playing)
                return;

            try
            {
                Events.OnPlaybackTick?.Invoke();
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }
        
        /// <summary>
        /// Called when playback is requested to start
        /// This spawns a coroutine that waits until a delay has passed to begin playing
        /// </summary>
        public void BeginPlayback()
        {
            if (Director.LastPlayState == PlayState.Paused)
            {
                Director.SetPlayState(PlayState.Playing);
                return;
            }

            try
            {
                m_film = Director.ActiveFilm;
                m_stage = m_film.Stages[0];

                if (m_playRoutine == null)
                    m_playRoutine = MelonCoroutines.Start(PlayRoutine()) as Coroutine;
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }

        /// <summary>
        /// Called before playback begins
        /// This resets the scene state and playhead
        /// </summary>
        public void OnPrePlayback()
        {
            try
            {
                ResetPlayhead();

                foreach (var castMember in Caster.Cast)
                {
                    castMember.OnSceneBegin();
                }

                foreach (var prop in Caster.Props)
                {
                    prop.OnSceneBegin();
                }
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }

        /// <summary>
        /// Called during playback
        /// </summary>
        public void OnPlay()
        {
            try
            {
                foreach (var actor in Caster.Cast)
                {
                    if (actor is Actor actorPlayer)
                    {
                        actorPlayer?.Microphone?.Playback();
                    }
                }
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }

        /// <summary>
        /// Called per playback tick
        /// </summary>
        public void OnPlaybackTick()
        {
            if (Director.PlayState == PlayState.Stopped || Director.PlayState == PlayState.Paused)
            {
                return;
            }

            try
            {
                AnimateAll();
                m_playbackTime += PlaybackRate * Time.deltaTime;
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }

        /// <summary>
        /// Called when playback is requested to stop
        /// </summary>
        public void OnStopPlayback()
        {
            try
            {
                foreach (Trackable castMember in Caster.Cast)
                {
                    if (castMember != null && castMember is Actor actorPlayer)
                    {
                        actorPlayer?.Microphone?.StopPlayback();
                    }
                }

                if (m_playRoutine != null)
                {
                    MelonCoroutines.Stop(m_playRoutine);
                    m_playRoutine = null;
                }
            }
            catch(Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }

        /// <summary>
        /// Manually seeks the playback head in the provided direction
        /// Negative seconds will reverse the playback 
        /// </summary>
        /// <param name="amount">The amount of seconds to seek the playback</param>
        public void Seek(float amount)
        {
            if (Director.PlayState != PlayState.Stopped)
                return;

            if (m_playbackTime <= 0f)
                m_playbackTime = 0f;

            if (m_playbackTime >= m_stage.Duration)
                m_playbackTime = m_stage.Duration;

            AnimateAll();

            m_playbackTime += amount;
        }

        /// <summary>
        /// Animates all tracked scene objects
        /// Call when playback head is seeked to make sure changes are applied!
        /// </summary>
        public void AnimateAll()
        {
            try
            {
                foreach (var castMember in Caster.Cast)
                    AnimateActor(castMember);

                foreach (var prop in Caster.Props)
                    AnimateProp(prop);
            }
            catch (Exception e)
            {
                Logging.ErrorDebug(e.ToString());
                Bootstrap.AnnounceError();
            }
        }
        
        /// <summary>
        /// Animates the provided actor
        /// </summary>
        /// <param name="actor">The actor to "act"</param>
        public void AnimateActor(Trackable actor)
        {
            if (actor != null)
                actor.Act();
        }

        /// <summary>
        /// Animates the provided prop
        /// </summary>
        /// <param name="prop">The prop to "act"</param>
        public void AnimateProp(Prop prop)
        {
            if (prop != null)
                prop.Act();
        }

        /// TODO: Is PlayRoutine() having a delay necessary?
        
        /// <summary>
        /// Playback coroutine. Supports a delay of any duration.
        /// </summary>
        /// <returns></returns>
        public IEnumerator PlayRoutine()
        {
            bool begun = false;

            for (int i = 0; i < m_film.Stages.Count; i++)
            {
                Events.OnPrePlayback?.Invoke();

                if (Director.PlayState == PlayState.Stopped)
                {
                    break;
                }

                m_stage = m_film.Stages[i];
                Director.SetStage(m_stage);

                if (!begun)
                {
                    for (Countdown = 0; Countdown < Settings.World.delay; Countdown++)
                    {
                        Events.OnTimerCountdown?.Invoke();
                        yield return new WaitForSeconds(1);
                    }

                    begun = true;
                }

                FeedbackSFX.BeepHigh();

                Events.OnPlay?.Invoke();

                while (Director.PlayState == PlayState.Playing || Director.PlayState == PlayState.Paused)
                {
                    // TODO: Replace this with WaitUntil to prevent Coroutine garbage?
                    while (Director.PlayState == PlayState.Paused)
                        yield return null;

                    if (PlaybackTime >= m_stage.Duration)
                        break;

                    Tick();

                    yield return null;
                }

                yield return null;
            }
            
            Events.OnStopPlayback?.Invoke();
        }
    }
}
