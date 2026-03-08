using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    /// <summary>
    /// Simple camera controller.
    /// </summary>
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FreeCamera(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public class Settings
        {
            public float slowSpeed;
            public float fastSpeed;
            public float maxSpeed;

            public float friction;

            public float fovChangeRate;
            public float fovChangeSmoothing;

            public float xShake;
            public float yShake;
            public float zShake;

            public float shakeMultiplier;
        }

        public Settings CameraSettings;

        private Vector3 m_shakeVector;

        private Vector3 m_wishDir = Vector3.zero;

        private bool m_fastCamera => Input.GetKey(KeyCode.LeftShift);

        private float m_currentSpeed = 0f;

        private Rigidbody m_rigidbody;

        private InputController m_inputControl = CameraRigManager.Instance.InputController;

        protected void Awake()
        {
            m_rigidbody = gameObject.AddComponent<Rigidbody>();
            m_rigidbody.useGravity = false;

            CameraSettings = new Settings()
            {
                slowSpeed = 5f,
                fastSpeed = 10f,
                maxSpeed = 15f,
                friction = 5f,
                xShake = 0f,
                yShake = 0f,
                zShake = 0f,
                shakeMultiplier = 0f
            };
        }

        protected void Update()
        {
            MoveUpdate();
            MouseUpdate();
        }

        private void MouseUpdate()
        {
            Vector3 mouseVector = m_inputControl.MouseMove();

            float shakeX = Mathf.Sin(Time.time) + Mathf.PerlinNoise(CameraSettings.xShake * Time.time, CameraSettings.yShake) * m_shakeVector.x;
            float shakeY = Mathf.Sin(Time.time) + Mathf.PerlinNoise(CameraSettings.xShake, 1f - CameraSettings.yShake * Time.time) * m_shakeVector.y;
            float shakeZ = shakeY * m_shakeVector.z;

            Vector3 rightVector = (Vector3.right * (mouseVector.y + (shakeX * CameraSettings.shakeMultiplier)));
            Vector3 upVector = (Vector3.up * (mouseVector.x + (shakeY * CameraSettings.shakeMultiplier)));
            Vector3 forwardVector = (Vector3.forward * (mouseVector.z + (shakeZ * CameraSettings.shakeMultiplier)));

            transform.rotation = Quaternion.Euler(rightVector + upVector + forwardVector);
        }

        private void MoveUpdate()
        {
            Vector3 inputVector = m_inputControl.KeyboardMove();

            Vector3.ClampMagnitude(m_wishDir, CameraSettings.maxSpeed);

            Transform t = CameraRigManager.Instance.Camera.transform;

            m_currentSpeed = m_fastCamera ? CameraSettings.fastSpeed : CameraSettings.slowSpeed;

            inputVector = Vector3.ClampMagnitude(inputVector, CameraSettings.maxSpeed);

            m_wishDir = ((t.right * inputVector.x) + (t.up * inputVector.y) + (t.forward * inputVector.z));
            m_rigidbody.AddForce(m_wishDir * m_currentSpeed);

            m_rigidbody.drag = CameraSettings.friction;
        }
    }
}