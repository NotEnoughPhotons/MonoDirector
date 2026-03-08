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

        private SmoothFollower m_smoothFollower;

        private void Awake()
        {
            m_smoothFollower = GetComponent<SmoothFollower>();
            m_smoothFollower.enabled = false;

            FOVController = gameObject.AddComponent<FOVController>();
            FollowCamera = gameObject.AddComponent<FollowCamera>();
            CameraVolume = gameObject.AddComponent<CameraVolume>();

            FOVController.SetCamera(GetComponent<Camera>());
        }

        private void Start()
        {
            FollowCamera.SetFollowTarget(m_smoothFollower.targetTransform);
        }
    }
}
