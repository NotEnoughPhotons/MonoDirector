using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Proxy;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundSourceTether(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private SoundSource3D m_source;

        private Grip m_tetherGrip;
        private Rigidbody m_tetherBody;
        private Rigidbody m_originalConnectedBody;
        private ConfigurableJoint m_tetherJoint;
        private GameObject m_tetherMesh;

        private Action<Hand> m_OnTetherGrabbed;
        private Action<Hand> m_OnTetherReleased;

        private Actor m_hoveredActor;
        private Actor m_attachedActor;

        private void Awake()
        {
            m_source = transform.parent.GetComponent<SoundSource3D>();

            m_OnTetherGrabbed = OnTetherGrabbed;
            m_OnTetherReleased = OnTetherReleased;

            m_tetherGrip = GetComponent<Grip>();
            m_tetherBody = GetComponent<Rigidbody>();
            m_tetherJoint = GetComponent<ConfigurableJoint>();
            m_tetherMesh = transform.Find("Sphere").gameObject;

            m_originalConnectedBody = m_tetherJoint.connectedBody;
        }

        private void OnEnable()
        {
            m_tetherGrip.attachedHandDelegate += m_OnTetherGrabbed;
            m_tetherGrip.detachedHandDelegate += m_OnTetherReleased;

            m_tetherJoint.connectedBody = m_originalConnectedBody;
        }

        private void OnDisable()
        {
            m_tetherGrip.attachedHandDelegate -= m_OnTetherGrabbed;
            m_tetherGrip.detachedHandDelegate -= m_OnTetherReleased;

            m_attachedActor.Microphone.AssignSound(null);

            m_source.Unmute();
        }

        protected void OnHandAttached(Hand hand)
        {
            if (m_attachedActor != null)
            {
                return;
            }

            m_tetherJoint.xMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.yMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.zMotion = ConfigurableJointMotion.Limited;
        }

        protected void OnHandDetached(Hand hand)
        {
            if (m_attachedActor != null)
            {
                return;
            }

            m_tetherJoint.xMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.yMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.zMotion = ConfigurableJointMotion.Limited;
        }
        private void OnTetherGrabbed(Hand hand)
        {
            if (m_attachedActor != null)
            {
                UnTether();
            }

            m_tetherJoint.xMotion = ConfigurableJointMotion.Free;
            m_tetherJoint.yMotion = ConfigurableJointMotion.Free;
            m_tetherJoint.zMotion = ConfigurableJointMotion.Free;
        }

        private void OnTetherReleased(Hand hand)
        {
            if (m_hoveredActor != null)
            {
                Tether();
            }

            if (m_attachedActor != null)
            {
                m_tetherJoint.xMotion = ConfigurableJointMotion.Free;
                m_tetherJoint.yMotion = ConfigurableJointMotion.Free;
                m_tetherJoint.zMotion = ConfigurableJointMotion.Free;
                return;
            }

            m_tetherJoint.xMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.yMotion = ConfigurableJointMotion.Limited;
            m_tetherJoint.zMotion = ConfigurableJointMotion.Limited;
        }

        private void OnTriggerEnter(Collider other)
        {
            MarrowBody body = other.GetComponent<MarrowBody>();

            if (body != null)
            {
                ActorProxy proxy = body.Entity.GetComponent<ActorProxy>();

                if (proxy != null)
                {
                    OnTetherTriggerEnter(proxy.Actor);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            MarrowBody body = other.GetComponent<MarrowBody>();

            if (body != null)
            {
                ActorProxy proxy = body.Entity.GetComponent<ActorProxy>();

                if (proxy != null)
                {
                    OnTetherTriggerExit(proxy.Actor);
                }
            }
        }

        private void OnTetherTriggerEnter(Actor actor)
        {
            m_hoveredActor = actor;
        }

        private void OnTetherTriggerExit(Actor actor)
        {
            m_hoveredActor = null;
        }

        private void Tether()
        {
            m_hoveredActor.Microphone.AssignSound(m_source.Clip);
            m_hoveredActor.Microphone.SetCorrectionMode(ActorSpeech.AudioCorrectionMode.NonCorrected);
            m_attachedActor = m_hoveredActor;

            m_tetherJoint.connectedBody = m_hoveredActor.ActorBody.Head.GetComponent<Rigidbody>();

            Main.feedbackSFX.LinkAudio();
            m_hoveredActor = null;

            m_source.Mute();
        }

        private void UnTether()
        {
            m_tetherJoint.connectedBody = m_originalConnectedBody;
            m_attachedActor.Microphone.AssignSound(null);
            m_attachedActor = null;
        }

        public void Hide()
        {
            // This code exists solely to keep the config joint alive.
            m_tetherMesh.SetActive(false);
            m_tetherGrip.enabled = false;
        }

        public void Show()
        {
            m_tetherMesh.SetActive(true);
            m_tetherGrip.enabled = true;
        }
    }
}
