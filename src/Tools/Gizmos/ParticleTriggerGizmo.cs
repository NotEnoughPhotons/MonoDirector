using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class ParticleTriggerGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        private VFXVolume m_volume;

        protected override void Awake()
        {
            base.Awake();

            m_volume = GetComponentInParent<VFXVolume>();
        }

        protected override void OnHandAttached(Hand hand)
        {
            m_body.isKinematic = false;

            m_joint.zMotion = ConfigurableJointMotion.Free;
            m_joint.yMotion = ConfigurableJointMotion.Free;
            m_joint.zMotion = ConfigurableJointMotion.Free;
        }

        protected override void OnHandDetached(Hand hand)
        {
            m_body.isKinematic = true;

            m_joint.zMotion = ConfigurableJointMotion.Locked;
            m_joint.yMotion = ConfigurableJointMotion.Locked;
            m_joint.zMotion = ConfigurableJointMotion.Locked;
        }

        protected override void OnPrimaryButtonDown()
        {
            m_volume.TriggerVFX();
        }
    }
}
