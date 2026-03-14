using UnityEngine;
using MelonLoader;
using Il2CppSLZ.Marrow.Interaction;
using UnityEngine.UI;
using NEP.MonoDirector.Core;
using System.Threading.Tasks.Sources;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageShelfSocket(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Rigidbody Body { get => m_body; }
        public StageReel Reel { get => m_reel; }
        public int Index => transform.GetSiblingIndex();
        public bool Empty { get => m_empty; }
        public bool IsSpawner { get => m_isSpawner; }
        public bool IsDisconnected { get => m_isDisconnected; }

        public event Action<StageReel> OnConnected;
        public event Action<StageReel> OnDisconnected;

        private StageReel m_reel;
        private StageReel m_hoveredReel;
        private Rigidbody m_body;
        private Image m_image;
        private bool m_empty;
        private bool m_isSpawner;
        private bool m_isDisconnected;

        private Color m_regularColor;
        private Color m_hoverColor;


        private void Awake()
        {
            m_image = transform.Find("Outline")?.GetComponent<Image>();
            m_body = GetComponent<Rigidbody>();

            m_regularColor = Color.white;
            m_hoverColor = Color.blue;

            m_empty = true;
            m_isSpawner = false;
        }

        private void OnEnable()
        {
            if (m_image == null)
            {
                return;
            }

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

        public void HoverOver()
        {
            if (m_image == null)
            {
                return;
            }

            m_image.color = m_hoverColor;
        }

        public void HoverAway()
        {
            if (m_image == null)
            {
                return;
            }

            m_image.color = m_regularColor;
        }

        public void Connect()
        {
            m_isDisconnected = false;

            OnConnected?.Invoke(m_reel);
        }

        public void Disconnect()
        {
            m_isDisconnected = true;

            OnDisconnected?.Invoke(m_reel);
        }
    }
}
