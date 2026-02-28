using Il2CppSLZ.Marrow;
using Il2CppTMPro;
using MelonLoader;
using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class LightRadiusGizmo(IntPtr ptr) : ToolGizmo(ptr)
    {
        public float Distance { get => m_distance; }

        private float m_distance = 0.1f;
        private TextMeshPro m_distanceText;

        protected override void Awake()
        {
            base.Awake();

            m_distanceText = transform.Find("Text").GetComponent<TextMeshPro>();
        }

        protected override void OnHandAttached(Hand hand)
        {
            m_body.isKinematic = false;
            m_joint.zMotion = ConfigurableJointMotion.Free;
        }

        protected override void OnHandDetached(Hand hand)
        {
            m_joint.connectedAnchor = transform.localPosition;
            m_joint.targetPosition = transform.localPosition;
            m_body.isKinematic = true;
            m_joint.zMotion = ConfigurableJointMotion.Limited;
        }

        private void Update()
        {
            float distance = Vector3.Distance(transform.position, transform.parent.position);

            m_distance = distance * 2f;
            m_distanceText.text = distance.ToString("0.00") + "m";
        }

        public override void Hide()
        {
            base.Hide();

            m_distanceText.gameObject.SetActive(false);
        }

        public override void Show()
        {
            base.Show();

            m_distanceText.gameObject.SetActive(true);
        }
    }
}
