using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Pool;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class VFXHolder(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public List<ParticleSystem> ParticleSystems { get => m_particleSystems; }

        private Rigidbody m_rigidbody;
        private Poolee m_poolee;
        private List<ParticleSystem> m_particleSystems;
        private Collider m_collider;

        private GameObject m_displayNameObject;
        private GameObject m_meshObject;
        private GameObject m_grip;

        private void Awake()
        {
            m_poolee = GetComponent<Poolee>();
            m_collider = GetComponent<Collider>();

            m_rigidbody = GetComponent<Rigidbody>();

            m_displayNameObject = transform.Find("DisplayName").gameObject;
            m_meshObject = transform.Find("Mesh").gameObject;
            m_grip = transform.Find("Grip").gameObject;

            m_particleSystems = new List<ParticleSystem>();

            Transform particleContainer = transform.Find("Particles");

            for (int i = 0; i < particleContainer.childCount; i++)
            {
                ParticleSystem particleSystem = particleContainer.GetChild(i).GetComponent<ParticleSystem>();

                if (particleSystem != null)
                {
                    m_particleSystems.Add(particleSystem);
                }
            }
        }

        public void Despawn()
        {
            m_poolee.Despawn();
        }

        public void Hide()
        {
            m_rigidbody.isKinematic = true;
            m_displayNameObject.SetActive(false);
            m_meshObject.SetActive(false);
            m_grip.SetActive(false);
            m_collider.enabled = false;
        }

        public void Show()
        {
            m_rigidbody.isKinematic = false;
            m_displayNameObject.SetActive(true);
            m_meshObject.SetActive(true);
            m_grip.SetActive(true);
            m_collider.enabled = true;
        }
    }
}
