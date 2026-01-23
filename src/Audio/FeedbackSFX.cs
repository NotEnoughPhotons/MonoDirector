using AudioImportLib;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FeedbackSFX(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private AudioSource source;

        private AudioClip sfx_preroll = API.LoadAudioClip(Constants.dirSFX + "preroll.wav");
        private AudioClip sfx_postroll = API.LoadAudioClip(Constants.dirSFX + "postroll.wav");
        private AudioClip sfx_beep = API.LoadAudioClip(Constants.dirSFX + "beep.wav");
        private AudioClip sfx_linkedaudio = API.LoadAudioClip(Constants.dirSFX + "linkaudio.wav");

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
        }

        private void OnEnable()
        {
            Events.OnStopRecording += Beep;
            Events.OnStopPlayback += Beep;
            Events.OnTimerCountdown += Beep;
        }

        private void OnDisable()
        {
            Events.OnStartRecording -= Preroll;
            Events.OnStopRecording -= Beep;

            Events.OnPlay -= Postroll;
            Events.OnStopPlayback -= Beep;

            Events.OnPreSnapshot -= Preroll;

            Events.OnTimerCountdown -= Beep;
        }

        public void Play(AudioClip clip)
        {
            source.clip = clip;
            source.PlayOneShot(clip);
        }

        private void Preroll()
        {
            Play(sfx_preroll);
        }

        public void Beep()
        {
            source.pitch = 1f;
            Play(sfx_beep);
        }

        public void BeepLow()
        {
            source.pitch = 0.5f;
            Play(sfx_beep);
        }

        public void BeepHigh()
        {
            source.pitch = 2f;
            Play(sfx_beep);
        }

        public void LinkAudio()
        {
            Play(sfx_linkedaudio);
        }

        private void Postroll() 
        {
            Play(sfx_postroll);
        }
    }
}
