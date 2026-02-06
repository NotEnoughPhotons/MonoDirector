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
    public class SoundSourceTether(IntPtr ptr) : ToolGizmo(ptr)
    {
        private SoundSource3D m_source;

        private Rigidbody m_originalConnectedBody;

        private Actor m_hoveredActor;
        private Actor m_attachedActor;

        protected override void Awake()
        {
            base.Awake();
            m_source = transform.parent.GetComponent<SoundSource3D>();

            m_originalConnectedBody = m_joint.connectedBody;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_joint.connectedBody = m_originalConnectedBody;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            m_attachedActor.Microphone.AssignSound(null);

            m_source.Unmute();
        }

        protected override void OnHandAttached(Hand hand)
        {
            if (m_attachedActor != null)
            {
                UnTether();
            }

            m_joint.xMotion = ConfigurableJointMotion.Free;
            m_joint.yMotion = ConfigurableJointMotion.Free;
            m_joint.zMotion = ConfigurableJointMotion.Free;
        }

        protected override void OnHandDetached(Hand hand)
        {
            if (m_hoveredActor != null)
            {
                Tether();
            }

            if (m_attachedActor != null)
            {
                m_joint.xMotion = ConfigurableJointMotion.Free;
                m_joint.yMotion = ConfigurableJointMotion.Free;
                m_joint.zMotion = ConfigurableJointMotion.Free;
                return;
            }

            m_joint.xMotion = ConfigurableJointMotion.Limited;
            m_joint.yMotion = ConfigurableJointMotion.Limited;
            m_joint.zMotion = ConfigurableJointMotion.Limited;
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

            m_joint.connectedBody = m_hoveredActor.ActorBody.Head.GetComponent<Rigidbody>();

            Main.feedbackSFX.LinkAudio();
            m_hoveredActor = null;

            m_source.Mute();
        }

        private void UnTether()
        {
            m_joint.connectedBody = m_originalConnectedBody;
            m_attachedActor.Microphone.AssignSound(null);
            m_attachedActor = null;
        }
    }
}
