using AudioImportLib;
using UnityEngine;

using UnityEngine.Audio;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

namespace NEP.MonoDirector.Audio
{
    public static class FeedbackSFX
    {
        private static bool m_init;

        private static AudioClip m_sfxPreroll;
        private static AudioClip m_sfxPostroll;
        private static AudioClip m_sfxBeep;
        private static AudioClip m_sfxLinkedAudio;

        internal static void Initialize()
        {
            if (!m_init)
            {
                m_sfxPreroll = BundleLoader.PreRollClip;
                m_sfxPostroll = BundleLoader.PostRollClip;
                m_sfxBeep = BundleLoader.BeepClip;
                m_sfxLinkedAudio = BundleLoader.LinkAudioClip;
                m_init = true;
            }

            Events.OnStopRecording += Beep;
            Events.OnStopPlayback += Beep;
            Events.OnTimerCountdown += Beep;
        }

        internal static void Shutdown()
        {
            Events.OnStopRecording -= Beep;
            Events.OnStopPlayback -= Beep;
            Events.OnTimerCountdown -= Beep;
        }

        public static void Play(AudioClip clip, float pitch = 1.0f)
        {
            if (clip == null)
            {
                return;
            }

            AudioMixerGroup mixer = BoneLib.Audio.UI;
            BoneLib.Audio.Play2DOneShot(clip, mixer, 1f, pitch);
        }

        private static void Preroll()
        {
            Play(m_sfxPreroll);
        }

        public static void Beep()
        {
            Play(m_sfxBeep, 1f);
        }

        public static void BeepLow()
        {
            Play(m_sfxBeep, 0.5f);
        }

        public static void BeepHigh()
        {
            Play(m_sfxBeep, 2f);
        }

        public static void LinkAudio()
        {
            Play(m_sfxLinkedAudio);
        }

        private static void Postroll() 
        {
            Play(m_sfxPostroll);
        }
    }
}
