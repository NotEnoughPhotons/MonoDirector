using UnityEngine;

using MelonLoader;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public abstract class ToolGizmo(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Rigidbody Body { get => m_body; }
        public ConfigurableJoint Joint { get => m_joint; }

        protected Grip m_grip;
        protected Rigidbody m_body;
        protected ConfigurableJoint m_joint;
        protected MeshRenderer m_tetherMesh;

        private Action<Hand> m_onHandAttached;
        private Action<Hand> m_onHandReleased;
        private Action<Hand> m_onHandAttachedUpdate;

        protected virtual void Awake()
        {
            m_onHandAttached += OnHandAttached;
            m_onHandReleased += OnHandDetached;
            m_onHandAttachedUpdate += OnHandAttachedUpdate;

            m_grip = GetComponent<Grip>();
            m_body = GetComponent<Rigidbody>();
            m_joint = GetComponent<ConfigurableJoint>();
            m_tetherMesh = GetComponentInChildren<MeshRenderer>();
        }

        protected virtual void OnEnable()
        {
            m_grip.attachedHandDelegate += m_onHandAttached;
            m_grip.detachedHandDelegate += m_onHandReleased;
            m_grip.attachedUpdateDelegate += m_onHandAttachedUpdate;
        }

        protected virtual void OnDisable()
        {
            m_grip.attachedHandDelegate -= m_onHandAttached;
            m_grip.detachedHandDelegate -= m_onHandReleased;
            m_grip.attachedUpdateDelegate -= m_onHandAttachedUpdate;
        }

        protected virtual void OnHandAttached(Hand hand)
        {

        }

        protected virtual void OnHandDetached(Hand hand)
        {

        }

        protected virtual void OnHandAttachedUpdate(Hand hand)
        {
            if (hand.Controller.GetSecondaryInteractionButtonDown())
            {
                OnSecondaryButtonPressed();
            }
        }

        protected virtual void OnSecondaryButtonPressed()
        {

        }

        public virtual void Hide()
        {
            m_tetherMesh.enabled = false;
            m_grip.enabled = false;
        }

        public virtual void Show()
        {
            m_tetherMesh.enabled = true;
            m_grip.enabled = true;
        }
    }
}
