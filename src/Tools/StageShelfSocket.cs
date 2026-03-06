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
        public bool Empty { get => m_empty; }

        private StageReel m_reel;
        private StageReel m_hoveredReel;
        private Image m_image;
        private bool m_empty;

        private Color m_regularColor;
        private Color m_hoverColor;

        private void Awake()
        {
            m_image = transform.Find("Outline").GetComponent<Image>();

            m_regularColor = Color.white;
            m_hoverColor = Color.blue;

            m_empty = true;
        }

        private void OnEnable()
        {
            m_image.color = m_regularColor;
        }

        public void SetReel(StageReel reel)
        {
            if (reel == null)
            {
                m_reel = null;
                m_empty = true;
                return;
            }

            m_reel = reel;
            m_empty = false;
        }

        public void OnHoverOver()
        {
            m_image.color = m_hoverColor;
        }

        public void OnHoverAway()
        {
            m_image.color = m_regularColor;
        }
    }
}
