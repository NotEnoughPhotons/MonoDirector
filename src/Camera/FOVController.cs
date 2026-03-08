using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FOVController(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public float fovChangeSmoothing = 10f;
        public float fovChangeRate = 4f;

        private Camera m_camera;

        private float m_fov = 90f;
        private float m_lastFOV = 0f;

        private void Update()
        {
            MouseFOV();
        }

        private void LateUpdate()
        {
            m_camera.fieldOfView = Mathf.Lerp(m_lastFOV, m_fov, fovChangeSmoothing * Time.deltaTime);
        }

        private void MouseFOV()
        {
            m_lastFOV = m_camera.fieldOfView;
            SetFOV(Input.GetAxisRaw("Mouse ScrollWheel") * fovChangeRate);
        }

        public void SetCamera(Camera camera)
        {
            this.m_camera = camera;
        }

        public void SetFOV(float fov)
        {
            m_fov -= fov;
        }
    }
}
