using UnityEngine;

using MelonLoader;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public abstract class ToolGizmo(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Rigidbody Body { get => m_body; }
        public ConfigurableJoint Joint { get => m_joint; }

        protected Grip m_grip;
        protected Rigidbody m_body;
        protected MarrowBody m_marrowBody;
        protected ConfigurableJoint m_joint;
        protected MeshRenderer m_mesh;
        protected MarrowJoint m_marrowJoint;

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
            m_mesh = GetComponentInChildren<MeshRenderer>();
            m_marrowJoint = GetComponent<MarrowJoint>();
            m_marrowBody = GetComponent<MarrowBody>();
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
            if (hand.GetIndexButtonDown())
            {
                OnPrimaryButtonDown();
            }

            if (hand.Controller.GetSecondaryInteractionButtonDown())
            {
                OnSecondaryButtonPressed();
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

        protected virtual void OnSecondaryButtonPressed()
        {

        }

        protected virtual void OnAButtonDown()
        {

        }

        protected virtual void OnBButtonDown()
        {

        }

        public virtual void Hide()
        {
            m_mesh.enabled = false;
            m_grip.enabled = false;

            foreach (Collider collider in m_marrowBody.Colliders)
            {
                collider.enabled = false;
            }
        }

        public virtual void Show()
        {
            m_mesh.enabled = true;
            m_grip.enabled = true;
            
            foreach (Collider collider in m_marrowBody.Colliders)
            {
                collider.enabled = true;
            }
        }
    }
}
