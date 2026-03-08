using NEP.MonoDirector.Patches;
using NEP.MonoDirector.State;

using UnityEngine;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Cameras
{
    public class CameraRigManager
    {
        public CameraRigManager(RigScreenOptions options)
        {
            RigScreenOptions = options;
            Start();
        }

        public static CameraRigManager Instance { get; private set; }

        public RigScreenOptions RigScreenOptions { get; private set; }

        public Camera Camera { get; private set; }
        public Camera ClonedCamera { get; private set; }

        public InputController InputController { get; private set; }
        public FreeCamera FreeCamera { get; private set; }
        public FollowCamera FollowCamera { get; private set; }

        public FOVController FOVController { get; private set; }
        public CameraDamp CameraDamp { get; private set; }
        public CameraVolume[] CameraVolumes { get; private set; }
        public CameraDisplay CameraDisplay { get; private set; }
        public SmoothFollower SmoothFollower { get; private set; }

        public RenderTexture CameraDisplayTexture { get; private set; }

        public CameraMode CameraMode
        {
            get => m_cameraMode;
            set
            {
                m_cameraMode = value;

                // Default spectator camera mode
                if (m_cameraMode == CameraMode.None)
                {
                    // Disable any effects that we have on the camera
                    FreeCamera.enabled = false;
                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;

                    // Override any effects
                    SmoothFollower.enabled = true;
                }

                // Free camera mode using WASD and the mouse
                if (m_cameraMode == CameraMode.Free)
                {
                    SmoothFollower.enabled = false;

                    FreeCamera.enabled = true;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;
                }

                // Modified spectator camera with smooth rotations and custom targets
                if (m_cameraMode == CameraMode.Head)
                {
                    SmoothFollower.enabled = false;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = true;

                    FreeCamera.enabled = false;
                }

                Events.OnCameraModeSet?.Invoke(m_cameraMode);
            }
        }

        public float CameraSmoothness
        {
            get => FollowCamera.delta;
            set => FollowCamera.delta = value;
        }

        public float MouseSensitivity
        {
            get => InputController.mouseSensitivity;
            set => InputController.mouseSensitivity = value;
        }

        public float MouseSmoothness
        {
            get => InputController.mouseSmoothness;
            set => InputController.mouseSmoothness = value;
        }

        public float SlowSpeed
        {
            get => FreeCamera.CameraSettings.slowSpeed;
            set => FreeCamera.CameraSettings.slowSpeed = value;
        }

        public float FastSpeed
        {
            get => FreeCamera.CameraSettings.fastSpeed;
            set => FreeCamera.CameraSettings.fastSpeed = value;
        }

        public float MaxSpeed
        {
            get => FreeCamera.CameraSettings.maxSpeed;
            set => FreeCamera.CameraSettings.maxSpeed = value;
        }

        public float Friction
        {
            get => FreeCamera.CameraSettings.friction;
            set => FreeCamera.CameraSettings.friction = value;
        }

        private GameObject m_cameraObject;
        private CameraMode m_cameraMode = CameraMode.None;

        private void Start()
        {
            Instance = this;
            InitializeCamera(RigScreenOptions);
        }

        private void InitializeCamera(RigScreenOptions screenOptions)
        {
            Camera = screenOptions.cam;
            m_cameraObject = Camera.gameObject;

            ClonedCamera = GameObject.Instantiate(m_cameraObject).GetComponent<Camera>();
            ClonedCamera.gameObject.SetActive(false);

            m_cameraObject.transform.parent = null;

            SmoothFollower = m_cameraObject.GetComponent<SmoothFollower>();
            InputController = m_cameraObject.AddComponent<InputController>();

            FreeCamera = m_cameraObject.AddComponent<FreeCamera>();
            FOVController = m_cameraObject.AddComponent<FOVController>();
            FollowCamera = m_cameraObject.AddComponent<FollowCamera>();
            CameraDamp = m_cameraObject.AddComponent<CameraDamp>();

            CameraVolumes = new CameraVolume[2]
            {
                m_cameraObject.AddComponent<CameraVolume>(),
                ClonedCamera.gameObject.AddComponent<CameraVolume>(),
            };

            //CameraDisplay = cameraObject.AddComponent<CameraDisplay>();

            FOVController.SetCamera(Camera);

            CameraMode = CameraMode.None;

            FollowCamera.SetFollowTarget(SmoothFollower.targetTransform);

            CameraDisplay = ClonedCamera.gameObject.AddComponent<CameraDisplay>();
        }

        public void EnableLensDistortion(bool enable)
        {
            foreach (var cameraVolume in CameraVolumes)
            {
                cameraVolume.LensDistortion.active = enable;
            }
        }

        public void EnableChromaticAbberation(bool enable)
        {
            foreach (var cameraVolume in CameraVolumes)
            {
                cameraVolume.ChromaticAberration.active = enable;
            }
        }
    }
}
