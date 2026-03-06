using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Interaction;
using UnityEngine.UI;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageShelfSocket(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public StageReel Reel { get => m_reel; }

        private StageReel m_reel;
        private Image m_image;

        private Color m_regularColor;
        private Color m_hoverColor;

        private void Awake()
        {
            m_image = transform.Find("Outline").GetComponent<Image>();

            m_regularColor = Color.white;
            m_hoverColor = Color.blue;
        }

        private void OnEnable()
        {
            m_image.color = m_regularColor;
        }

        public void SetReel(StageReel reel)
        {
            m_reel = reel;
        }

        public void OnHoverOver()
        {
            m_image.color = m_hoverColor;
        }

        public void OnHoverAway()
        {
            m_image.color = m_regularColor;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if (!collider.GetComponent<StageReel>())
            {
                return;
            }

            OnHoverOver();
        }

        private void OnTriggerExit(Collider collider)
        {
            if (!collider.GetComponent<StageReel>())
            {
                return;
            }

            OnHoverAway();
        }
    }
}
