using AudioImportLib;
using UnityEngine;

using BoneLib;
using UnityEngine.Audio;

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
                m_sfxPreroll = API.LoadAudioClip(Constants.dirSFX + "preroll.wav");
                m_sfxPostroll = API.LoadAudioClip(Constants.dirSFX + "postroll.wav");
                m_sfxBeep = API.LoadAudioClip(Constants.dirSFX + "beep.wav");
                m_sfxLinkedAudio = API.LoadAudioClip(Constants.dirSFX + "linkaudio.wav");
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
