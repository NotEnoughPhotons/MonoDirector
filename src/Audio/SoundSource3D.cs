using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Audio
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundSource3D(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private AudioSource source;
        private GameObject sprite;
        private GameObject frame;
        private Grip grip;
        private Rigidbody rb;

        private void Awake()
        {
            source = GetComponent<AudioSource>();
            sprite = transform.Find("Sprite").gameObject;
            frame = transform.Find("Sprite/Frame").gameObject;
            grip = transform.Find("Grip").GetComponent<Grip>();
            grip.GetComponent<Collider>().isTrigger = true;
            rb = GetComponent<Rigidbody>();

            source.spatialBlend = 1f;
        }

        private void OnEnable()
        {
            Events.OnPlayStateSet += OnPlayStateSet;
            Events.OnStartRecording += OnStartRecording;
            Events.OnPlay += OnPlay;
            Events.OnStopRecording += OnStopRecording;
            Events.OnStopPlayback += OnStopPlayback;

            grip.attachedHandDelegate += new System.Action<Hand>(AttachedHand);
            grip.detachedHandDelegate += new System.Action<Hand>(DetachedHand);
        }

        private void OnDisable()
        {
            Events.OnPlayStateSet -= OnPlayStateSet;
            Events.OnStartRecording -= OnStartRecording;
            Events.OnPlay -= OnPlay;
            Events.OnStopRecording -= OnStopRecording;
            Events.OnStopPlayback -= OnStopPlayback;

            grip.attachedHandDelegate -= new System.Action<Hand>(AttachedHand);
            grip.detachedHandDelegate -= new System.Action<Hand>(DetachedHand);
        }

        private void OnTriggerEnter(Collider other)
        {
            Main.Logger.Msg(other.name);
            SoundHolder soundHolder = other.GetComponent<SoundHolder>();

            if (soundHolder == null)
            {
                return;
            }

            source.clip = soundHolder.GetSound();
            soundHolder.gameObject.SetActive(false);
            Main.feedbackSFX.LinkAudio();
        }

        private void AttachedHand(Hand hand)
        {
            rb.isKinematic = false;
        }

        private void DetachedHand(Hand hand)
        {
            rb.isKinematic = true;
        }

        private void OnPlayStateSet(PlayState playState)
        {
            if (playState == PlayState.Preplaying
            || playState == PlayState.Playing)
            {
                HideVisuals();
            }
            else
            {
                ShowVisuals();
            }
        }

        private void OnStartRecording()
        {
            source.Play();
        }

        private void OnPlay()
        {
            source.Play();
        }

        private void OnStopPlayback()
        {
            source.Stop();
        }

        private void OnStopRecording()
        {
            source.Stop();
        }

        private void ShowVisuals()
        {
            sprite.SetActive(true);
            frame.SetActive(true);
        }

        private void HideVisuals()
        {
            sprite.SetActive(false);
            frame.SetActive(false);
        }
    }
}