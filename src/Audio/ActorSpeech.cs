using UnityEngine;
using UnityEngine.Audio;

using System.Linq;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorSpeech(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public enum AudioCorrectionMode
        {
            NonCorrected,
            Corrected
        }

        private MarrowAvatar avatar;

        private float jawLerp = 16f;
        private float lowBand = 0f;
        private float highBand = 44100f;

        private AudioClip clip;
        private AudioSource source;

        private Transform head;
        private Transform jaw;

        private Vector3 initialJawRotation;

        private Spectrum spectrum;

        private float desyncTolerance = 3f;

        private bool beginPlay;

        private AudioCorrectionMode mode;

        private void Awake()
        {
            source = gameObject.AddComponent<AudioSource>();
            source.spatialBlend = 1f;

            spectrum = gameObject.AddComponent<Spectrum>();
            spectrum.source = source;
            spectrum.freqLow = lowBand;
            spectrum.freqHigh = highBand;
        }

        public void SetAvatar(MarrowAvatar avatar)
        {
            this.avatar = avatar;

            head = avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            jaw = avatar.animator.GetBoneTransform(HumanBodyBones.Jaw);

            if(jaw != null)
            {
                initialJawRotation = new Vector3(jaw.localEulerAngles.x, jaw.localEulerAngles.y, jaw.localEulerAngles.z);
            }

            transform.parent = jaw != null ? jaw : head;
            transform.localPosition = Vector3.zero;
        }

        public void AssignSound(AudioClip sound)
        {
            this.clip = sound;
            source.clip = clip;
        }

        public void UpdateJaw()
        {
            if(jaw == null)
            {
                return;
            }

            Quaternion lastJawRotation = jaw.localRotation;
            Quaternion nextJawRotation = Quaternion.Euler(new Vector3(initialJawRotation.x, initialJawRotation.y, initialJawRotation.z + spectrum.BandVol(lowBand, highBand) * 10000f));
            jaw.localRotation = Quaternion.Slerp(lastJawRotation, nextJawRotation, jawLerp * Time.deltaTime);
        }

        public void Playback()
        {
            if (!beginPlay)
            {
                source.Play();
                beginPlay = true;
                return;
            }

            if (mode == AudioCorrectionMode.Corrected)
            {
                // special thanks to wnp and someone somewhere for this suggestion
                // stops desyncs of mic recordings, and also allows for slow motion playback!
                float tolerance = Time.deltaTime * desyncTolerance;
                float time = Mathf.Abs(source.time - Core.Playback.Instance.PlaybackTime);

                if (time > tolerance)
                {
                    source.time = Core.Playback.Instance.PlaybackTime;
                    source.pitch = Time.timeScale * Core.Playback.Instance.PlaybackRate;
                }
            }
        }

        public void SetCorrectionMode(AudioCorrectionMode correctionMode)
        {
            mode = correctionMode;
        }

        public void StopPlayback()
        {
            source.Stop();
            beginPlay = false;
        }

        public void RecordMicrophone()
        {
            if (!Settings.World.useMicrophone)
            {
                return;
            }

            AssignSound(Microphone.Start(null, false, 60 * 4, 44100));
        }

        public void StopRecording()
        {
            Microphone.End(null);
            source.clip = clip;
        }
    }
}
