using Il2CppSLZ.Bonelab;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraDisplay(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public FOVController FOVController { get; private set; }
        public FollowCamera FollowCamera { get; private set; }
        public CameraVolume CameraVolume { get; private set; }

        private SmoothFollower smoothFollower;

        private void Awake()
        {
            smoothFollower = GetComponent<SmoothFollower>();
            smoothFollower.enabled = false;

            FOVController = gameObject.AddComponent<FOVController>();
            FollowCamera = gameObject.AddComponent<FollowCamera>();
            CameraVolume = gameObject.AddComponent<CameraVolume>();

            FOVController.SetCamera(GetComponent<Camera>());
        }

        private void Start()
        {
            FollowCamera.SetFollowTarget(smoothFollower.targetTransform);
        }
    }
}
