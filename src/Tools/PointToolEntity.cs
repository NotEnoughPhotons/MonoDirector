using System;

using Il2CppSLZ.Marrow;

using NEP.MonoDirector.State;

using UnityEngine;

using MelonLoader;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class PointToolEntity(IntPtr ptr) : DirectedComponent(ptr)
    {
        private Rigidbody m_rigidbody;
        private GameObject m_frame;
        private Grip m_grip;

        private Action<Hand> m_OnHandAttached;
        private Action<Hand> m_OnHandDetached;

        protected virtual void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_frame = transform.Find("Frame").gameObject;
            m_grip = transform.Find("Grip").GetComponent<Grip>();

            m_OnHandAttached = new Action<Hand>(OnHandAttached);
            m_OnHandDetached = new Action<Hand>(OnHandDetached);
        }

        protected virtual void OnEnable()
        {
            base.OnEnable();

            m_grip.attachedHandDelegate += m_OnHandAttached;
            m_grip.detachedHandDelegate += m_OnHandDetached;
        }

        protected virtual void OnDisable()
        {
            base.OnDisable();

            m_grip.attachedHandDelegate -= m_OnHandAttached;
            m_grip.detachedHandDelegate -= m_OnHandDetached;
        }

        protected virtual void OnPlayStateSet(PlayState playState)
        {
            if (playState == PlayState.Preplaying
            || playState == PlayState.Playing
            || playState == PlayState.Stopped)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected virtual void OnHandAttached(Hand hand)
        {
            m_rigidbody.isKinematic = false;
        }

        protected virtual void OnHandDetached(Hand hand)
        {
            m_rigidbody.isKinematic = true;
        }

        protected virtual void OnPrimaryButtonDown()
        {

        }

        protected virtual void OnTriggerGripUpdate()
        {

        }

        protected virtual void Show()
        {
            m_frame.SetActive(true);
        }

        protected virtual void Hide()
        {
            m_frame.SetActive(false);
        }
    }
}
