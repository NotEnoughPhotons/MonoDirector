using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class ToolEntity(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Grip m_grip;

        private Action<Hand> m_OnHandAttached;
        private Action<Hand> m_OnHandDetached;
        private Action<Hand> m_OnTriggerGripUpdate;

        protected virtual void Awake()
        {
            m_grip = transform.Find("Grip").GetComponent<Grip>();

            m_OnHandAttached = OnHandAttached;
            m_OnHandDetached = OnHandDetached;
            m_OnTriggerGripUpdate = OnTriggerGripUpdate;
        }

        protected virtual void OnEnable()
        {
            m_grip.attachedHandDelegate += m_OnHandAttached;
            m_grip.detachedHandDelegate += m_OnHandDetached;
            m_grip.attachedUpdateDelegate += m_OnTriggerGripUpdate;
        }

        protected virtual void OnDisable()
        {
            m_grip.attachedHandDelegate -= m_OnHandAttached;
            m_grip.detachedHandDelegate -= m_OnHandDetached;
            m_grip.attachedUpdateDelegate -= m_OnTriggerGripUpdate;
        }

        protected virtual void OnHandAttached(Hand hand)
        {

        }

        protected virtual void OnHandDetached(Hand hand)
        {

        }

        protected virtual void OnTriggerGripUpdate(Hand hand)
        {
            if (hand.GetIndexButtonDown())
            {
                OnPrimaryButtonDown();
            }

            if (hand.Controller.GetAButtonDown())
            {
                OnAButtonDown();
            }

            if (hand.Controller.GetBButtonDown())
            {
                OnBButtonDown();
            }
        }

        protected virtual void OnPrimaryButtonDown()
        {

        }

        protected virtual void OnAButtonDown()
        {

        }

        protected virtual void OnBButtonDown()
        {

        }
    }
}
