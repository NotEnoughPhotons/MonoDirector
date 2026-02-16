using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class ParticleTriggerGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        private enum TriggerMode
        {
            NotRecording,
            Recording
        }
        
        private VFXVolume m_volume;
        private VFXCharge m_charge;
        private TriggerMode m_triggerMode;

        private Color m_activeColor;
        private Color m_inactiveColor;
        
        protected override void Awake()
        {
            base.Awake();

            m_volume = GetComponentInParent<VFXVolume>();
            m_charge = GetComponentInParent<VFXCharge>();

            m_activeColor = Color.red;
            m_inactiveColor = Color.white;
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

            // m_joint.zMotion = ConfigurableJointMotion.Locked;
            // m_joint.yMotion = ConfigurableJointMotion.Locked;
            // m_joint.zMotion = ConfigurableJointMotion.Locked;
        }

        protected override void OnBButtonDown()
        {
            if (m_triggerMode == TriggerMode.NotRecording)
            {
                m_triggerMode = TriggerMode.Recording;
            }
            else
            {
                m_triggerMode = TriggerMode.NotRecording;
            }
            
            if (m_triggerMode == TriggerMode.NotRecording)
            {
                m_tetherMesh.material.SetColor("_Emission", m_inactiveColor);
            }
            else
            {
                m_tetherMesh.material.SetColor("_Emission", m_activeColor);
            }
        }

        protected override void OnPrimaryButtonDown()
        {
            if (m_volume != null)
            {
                m_volume.TriggerVFX();
                return;
            }

            if (m_triggerMode == TriggerMode.Recording)
            {
                m_charge.RecordTrigger();
            }
            
            m_charge.TriggerVFX();
        }
    }
}
