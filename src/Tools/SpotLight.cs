using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SpotLight(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static List<SpotLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        private Rigidbody rb;

        private GameObject sprite;

        private Grip lightGrip;

        private GameObject arrow;

        private void Awake()
        {
            ComponentCache = new List<SpotLight>();
            rb = GetComponent<Rigidbody>();
            sprite = transform.Find("Sprite").gameObject;
            lightGrip = transform.Find("Grip").GetComponent<Grip>();

            arrow = transform.Find("arrow").gameObject;
        
            lightGrip.attachedHandDelegate += new System.Action<Hand>((hand) => AttachedHand(hand));
            lightGrip.detachedHandDelegate += new System.Action<Hand>((hand) => DetachedHand(hand));
        }

        private void OnEnable()
        {
            Events.OnPlayStateSet += OnPlayStateSet;
            ComponentCache.Add(this);
        }

        private void OnDisable()
        {
            Events.OnPlayStateSet -= OnPlayStateSet;
            ComponentCache.Remove(this);
        }

        private void AttachedHand(Hand hand)
        {
            rb.isKinematic = false;
        }

        private void DetachedHand(Hand hand)
        {
            rb.isKinematic = true;
        }

        private void ShowVisuals()
        {
            sprite.SetActive(true);
            arrow.SetActive(true);
        }

        private void HideVisuals()
        {
            sprite.SetActive(false);
            arrow.SetActive(false);
        }

        private void OnPlayStateSet(PlayState playState)
        {
            if (playState == PlayState.Preplaying 
            || playState == PlayState.Playing 
            || playState == PlayState.Stopped)
            {
                HideVisuals();
            }
            else
            {
                ShowVisuals();
            }
        }
    }
}