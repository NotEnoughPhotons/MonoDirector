using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Pool;
using MelonLoader;
using NEP.MonoDirector.Audio;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class VFXCharge(IntPtr ptr) : PointToolEntity(ptr)
    {
        private GameObject m_boundsFrame;
        private VFXHolder m_holder;
        private List<ParticleSystem> m_particleSystems;
        private ParticleTriggerGizmo m_triggerGizmo;

        private VFXVolumeSizeGizmo m_radiusGizmo;

        protected override void Awake()
        {
            base.Awake();

            m_particleSystems = new List<ParticleSystem>();

            m_holder = null;
            m_triggerGizmo = transform.Find("ParticleTriggerGizmo").GetComponent<ParticleTriggerGizmo>();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnparentVFXHolder();
            m_holder.Despawn();
            m_holder = null;
        }

        protected override void OnHandAttached(Hand hand)
        {
            base.OnHandAttached(hand);

            m_triggerGizmo.Body.isKinematic = false;

            m_radiusGizmo.Body.isKinematic = false;
        }
        protected override void OnHandDetached(Hand hand)
        {
            if (GetAttachedHands() > 1)
            {
                return;
            }

            base.OnHandDetached(hand);

            m_triggerGizmo.Body.isKinematic = true;

            m_radiusGizmo.Body.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            VFXHolder holder = other.GetComponent<VFXHolder>();

            if (holder == null)
            {
                return;
            }

            m_holder = holder;

            LinkVFX(m_holder);
        }

        private void LinkVFX(VFXHolder holder)
        {
            SetParticles(m_holder.ParticleSystems);
            ReparentVFXHolder();
            FeedbackSFX.LinkAudio();
        }

        private void ReparentVFXHolder()
        {
            m_holder.transform.SetParent(transform);
            m_holder.transform.localPosition = Vector3.zero;
            m_holder.transform.localRotation = Quaternion.identity;
            m_holder.Hide();
        }

        private void UnparentVFXHolder()
        {
            if (m_holder == null)
            {
                return;
            }

            m_holder.Show();
            m_holder.transform.SetParent(null);
        }

        public void SetParticles(List<ParticleSystem> particles)
        {
            m_particleSystems = particles;
        }

        public void TriggerVFX()
        {
            foreach (var particleSystem in m_particleSystems)
            {
                particleSystem.Play();
            }
        }
    }
}
