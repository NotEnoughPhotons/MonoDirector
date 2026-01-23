using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FOVController(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public float fovChangeSmoothing = 10f;
        public float fovChangeRate = 4f;

        private Camera camera;

        private float fieldOfView = 90f;
        private float lastFieldOfView = 0f;

        private void Update()
        {
            MouseFOV();
        }

        private void LateUpdate()
        {
            camera.fieldOfView = Mathf.Lerp(lastFieldOfView, fieldOfView, fovChangeSmoothing * Time.deltaTime);
        }

        private void MouseFOV()
        {
            lastFieldOfView = camera.fieldOfView;
            SetFOV(Input.GetAxisRaw("Mouse ScrollWheel") * fovChangeRate);
        }

        public void SetCamera(Camera camera)
        {
            this.camera = camera;
        }

        public void SetFOV(float fov)
        {
            fieldOfView -= fov;
        }
    }
}
