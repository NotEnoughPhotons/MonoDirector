using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Pool;
using MelonLoader;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
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
        private Prop m_prop;

        private VFXVolumeSizeGizmo m_radiusGizmo;

        protected override void Awake()
        {
            base.Awake();

            m_particleSystems = new List<ParticleSystem>();

            m_holder = null;
            m_triggerGizmo = transform.Find("ParticleTriggerGizmo").GetComponent<ParticleTriggerGizmo>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            BuildProp();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DestroyProp();
            UnparentVFXHolder();
            m_holder.Despawn();
            m_holder = null;
        }

        protected override void OnHandAttached(Hand hand)
        {
            base.OnHandAttached(hand);

            m_triggerGizmo.Body.isKinematic = false;
        }
        
        protected override void OnHandDetached(Hand hand)
        {
            if (GetAttachedHands() > 1)
            {
                return;
            }

            base.OnHandDetached(hand);

            m_triggerGizmo.Body.isKinematic = true;
        }

        protected override void Show()
        {
            base.Show();
            m_triggerGizmo.Show();
        }

        protected override void Hide()
        {
            base.Hide();
            m_triggerGizmo.Hide();
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
        
        public void BuildProp()
        {
            if (m_prop == null)
            {
                m_prop = gameObject.AddComponent<Prop>();
                Director.RecordingProps.Add(m_prop);
            }
        }

        public void DestroyProp()
        {
            if (m_prop != null)
            {
                Director.RecordingProps.Remove(m_prop);

                if (Director.WorldProps.Contains(m_prop))
                {
                    Director.WorldProps.Remove((m_prop));
                }
                
                Destroy(m_prop);
                m_prop = null;
            }
        }

        public void RecordTrigger()
        {
            m_prop.RecordAction(() => TriggerVFX());
        }

        public void SetParticles(List<ParticleSystem> particles)
        {
            m_particleSystems = particles;
        }

        public void TriggerVFX()
        {
            Logging.Msg("TriggerVFX");
            foreach (var particleSystem in m_particleSystems)
            {
                particleSystem.Play();
            }
        }
    }
}
