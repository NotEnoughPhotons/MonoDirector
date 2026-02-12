using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using MelonLoader;
using NEP.MonoDirector.State;
using System;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class PointToolEntity(IntPtr ptr) : DirectedComponent(ptr)
    {
        protected Rigidbody m_rigidbody;
        protected GameObject m_frame;
        protected Grip m_grip;

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

        protected override void OnEnable()
        {
            base.OnEnable();

            m_grip.attachedHandDelegate += m_OnHandAttached;
            m_grip.detachedHandDelegate += m_OnHandDetached;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_grip.attachedHandDelegate -= m_OnHandAttached;
            m_grip.detachedHandDelegate -= m_OnHandDetached;
        }

        protected override void OnPlayStateSet(PlayState playState)
        {
            if (playState == PlayState.Preplaying
            || playState == PlayState.Playing)
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
            if (GetAttachedHands() > 1)
            {
                return;
            }

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
            m_grip.enabled = true;
        }

        protected virtual void Hide()
        {
            m_frame.SetActive(false);
            m_grip.enabled = false;
        }

        protected int GetAttachedHands()
        {
            return m_grip.attachedHands.Count;
        }
    }
}
