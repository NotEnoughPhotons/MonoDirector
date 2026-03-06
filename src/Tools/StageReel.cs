using UnityEngine;
using MelonLoader;
using NEP.MonoDirector.Core;
using Il2CppTMPro;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageReel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Stage m_stage;
        private TextMeshPro m_title;
        private ConfigurableJoint m_joint;
        private StageShelfSocket m_socket;
        private Grip m_grip;

        private Action<Hand> m_onHandAttached;
        private Action<Hand> m_onHandReleased;

        private void Awake()
        {
            m_title = transform.Find("Text").GetComponent<TextMeshPro>();
            m_joint = GetComponent<ConfigurableJoint>();
            m_grip = transform.Find("Grip/Box").GetComponent<Grip>();

            m_onHandAttached = OnHandAttached;
            m_onHandReleased = OnHandReleased;
        }

        protected virtual void OnEnable()
        {
            m_grip.attachedHandDelegate += m_onHandAttached;
            m_grip.detachedHandDelegate += m_onHandReleased;
        }

        protected virtual void OnDisable()
        {
            m_grip.attachedHandDelegate -= m_onHandAttached;
            m_grip.detachedHandDelegate -= m_onHandReleased;
        }

        public void SetStage(Stage stage)
        {
            m_stage = stage;
            m_title.text = m_stage.Name;
        }

        public void AttachToSocket(StageShelfSocket socket)
        {
            m_socket = socket;

            m_joint.connectedBody = m_socket.GetComponent<Rigidbody>();
            m_joint.xMotion = ConfigurableJointMotion.Locked;
            m_joint.yMotion = ConfigurableJointMotion.Locked;
            m_joint.zMotion = ConfigurableJointMotion.Locked;
            m_joint.angularXMotion = ConfigurableJointMotion.Locked;
            m_joint.angularYMotion = ConfigurableJointMotion.Locked;
            m_joint.angularZMotion = ConfigurableJointMotion.Locked;
            m_joint.autoConfigureConnectedAnchor = false;
            m_joint.connectedAnchor = Vector3.zero;
            transform.localPosition = Vector3.zero;
            transform.rotation = Quaternion.identity;
        }

        public void DetachFromSocket()
        {
            m_joint.xMotion = ConfigurableJointMotion.Free;
            m_joint.yMotion = ConfigurableJointMotion.Free;
            m_joint.zMotion = ConfigurableJointMotion.Free;
            m_joint.angularXMotion = ConfigurableJointMotion.Free;
            m_joint.angularYMotion = ConfigurableJointMotion.Free;
            m_joint.angularZMotion = ConfigurableJointMotion.Free;
        }

        private void OnHandAttached(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
            {
                return;
            }

            DetachFromSocket();
        }

        private void OnHandReleased(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
            {
                return;
            }

            AttachToSocket(m_socket);
        }
    }
}
