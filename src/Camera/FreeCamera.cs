using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    /// <summary>
    /// Simple camera controller.
    /// </summary>
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FreeCamera : MonoBehaviour
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

        public FreeCamera(System.IntPtr ptr) : base(ptr) { }

        public Settings CameraSettings;

        private Vector3 shakeVector;

        private Vector3 wishDir = Vector3.zero;

        private bool fastCamera => Input.GetKey(KeyCode.LeftShift);

        private float currentSpeed = 0f;

        private Rigidbody rigidbody;

        private InputController inputController = CameraRigManager.Instance.InputController;

        protected void Awake()
        {
            rigidbody = gameObject.AddComponent<Rigidbody>();
            rigidbody.useGravity = false;

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
            Vector3 mouseVector = inputController.MouseMove();

            float shakeX = Mathf.Sin(Time.time) + Mathf.PerlinNoise(CameraSettings.xShake * Time.time, CameraSettings.yShake) * shakeVector.x;
            float shakeY = Mathf.Sin(Time.time) + Mathf.PerlinNoise(CameraSettings.xShake, 1f - CameraSettings.yShake * Time.time) * shakeVector.y;
            float shakeZ = shakeY * shakeVector.z;

            Vector3 rightVector = (Vector3.right * (mouseVector.y + (shakeX * CameraSettings.shakeMultiplier)));
            Vector3 upVector = (Vector3.up * (mouseVector.x + (shakeY * CameraSettings.shakeMultiplier)));
            Vector3 forwardVector = (Vector3.forward * (mouseVector.z + (shakeZ * CameraSettings.shakeMultiplier)));

            transform.rotation = Quaternion.Euler(rightVector + upVector + forwardVector);
        }

        private void MoveUpdate()
        {
            Vector3 inputVector = inputController.KeyboardMove();

            Vector3.ClampMagnitude(wishDir, CameraSettings.maxSpeed);

            Transform t = CameraRigManager.Instance.Camera.transform;

            currentSpeed = fastCamera ? CameraSettings.fastSpeed : CameraSettings.slowSpeed;

            inputVector = Vector3.ClampMagnitude(inputVector, CameraSettings.maxSpeed);

            wishDir = ((t.right * inputVector.x) + (t.up * inputVector.y) + (t.forward * inputVector.z));
            rigidbody.AddForce(wishDir * currentSpeed);

            rigidbody.drag = CameraSettings.friction;
        }
    }
}