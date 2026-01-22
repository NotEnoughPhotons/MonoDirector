using NEP.MonoDirector.Patches;
using NEP.MonoDirector.State;

using UnityEngine;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Cameras
{
    public class CameraRigManager
    {
        public CameraRigManager()
        {
            RigManager rigManager = Constants.RigManager;
            RigScreenOptions screenOptions = rigManager.GetComponent<RigScreenOptions>();
            RigScreenOptions = screenOptions;
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
            get => cameraMode;
            set
            {
                cameraMode = value;

                // Default spectator camera mode
                if (cameraMode == CameraMode.None)
                {
                    // Disable any effects that we have on the camera
                    FreeCamera.enabled = false;
                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;

                    // Override any effects
                    SmoothFollower.enabled = true;
                }

                // Free camera mode using WASD and the mouse
                if (cameraMode == CameraMode.Free)
                {
                    SmoothFollower.enabled = false;

                    FreeCamera.enabled = true;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = false;
                }

                // Modified spectator camera with smooth rotations and custom targets
                if (cameraMode == CameraMode.Head)
                {
                    SmoothFollower.enabled = false;

                    CameraDamp.enabled = false;
                    FollowCamera.enabled = true;

                    FreeCamera.enabled = false;
                }

                Events.OnCameraModeSet?.Invoke(cameraMode);
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

        private GameObject cameraObject;
        private CameraMode cameraMode;

        private GameObject cameraModel;
        private MeshRenderer cameraRenderer;

        private void Start()
        {
            Instance = this;
            InitializeCamera(RigScreenOptions);
        }

        private void InitializeCamera(RigScreenOptions screenOptions)
        {
            Camera = screenOptions.cam;
            cameraObject = Camera.gameObject;

            ClonedCamera = GameObject.Instantiate(cameraObject).GetComponent<Camera>();
            ClonedCamera.gameObject.SetActive(false);

            cameraObject.transform.parent = null;

            SmoothFollower = cameraObject.GetComponent<SmoothFollower>();
            InputController = cameraObject.AddComponent<InputController>();

            FreeCamera = cameraObject.AddComponent<FreeCamera>();
            FOVController = cameraObject.AddComponent<FOVController>();
            FollowCamera = cameraObject.AddComponent<FollowCamera>();
            CameraDamp = cameraObject.AddComponent<CameraDamp>();

            CameraVolumes = new CameraVolume[2]
            {
                cameraObject.AddComponent<CameraVolume>(),
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
