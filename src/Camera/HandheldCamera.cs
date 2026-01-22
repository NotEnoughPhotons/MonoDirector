using NEP.MonoDirector.State;

using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera : MonoBehaviour
    {
        public HandheldCamera(System.IntPtr ptr) : base(ptr) { }

        private CylinderGrip leftHandle;
        private CylinderGrip rightHandle;

        private Transform leftHandleTransform;
        private Transform rightHandleTransform;

        private Camera sensorCamera;

        private GameObject backViewfinderScreen;
        private GameObject frontViewfinderScreen;
        private GameObject displayScreen;

        private Rigidbody cameraRigidbody;

        private RenderTexture displayTexture  => sensorCamera.targetTexture;

        private void Awake()
        {
            leftHandleTransform = transform.Find("Grips/Left Handle");
            rightHandleTransform = transform.Find("Grips/Right Handle");

            sensorCamera = transform.Find("Sensor").GetComponent<Camera>();
            backViewfinderScreen = transform.Find("Viewfinder_Back").gameObject;
            frontViewfinderScreen = transform.Find("Viewfinder_Front").gameObject;
            displayScreen = transform.Find("Screen").gameObject;

            leftHandle = leftHandleTransform.GetComponent<CylinderGrip>();
            rightHandle = rightHandleTransform.GetComponent<CylinderGrip>();

            cameraRigidbody = GetComponent<Rigidbody>();
        }

        private void OnEnable()
        {
            Events.OnCameraModeSet += OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);
            leftHandle.detachedHandDelegate += new System.Action<Hand>(LeftHandDetached);
            leftHandle.detachedHandDelegate += new System.Action<Hand>(RightHandDetached);
        }

        private void OnDisable()
        {
            Events.OnCameraModeSet -= OnCameraModeChanged;

            leftHandle.attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            rightHandle.attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);
            leftHandle.detachedHandDelegate -= new System.Action<Hand>(LeftHandDetached);
            leftHandle.detachedHandDelegate -= new System.Action<Hand>(RightHandDetached);
        }

        private void OnCameraModeChanged(CameraMode mode)
        {
            if(mode == CameraMode.Handheld)
            {
                displayScreen.active = true;
                backViewfinderScreen.active = true;
                frontViewfinderScreen.active = true;

                CameraRigManager.Instance.ClonedCamera.targetTexture = displayTexture;
                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(true);
                CameraRigManager.Instance.FollowCamera.SetFollowTarget(sensorCamera.transform);
                CameraRigManager.Instance.CameraDisplay.FollowCamera.SetFollowTarget(sensorCamera.transform);
            }
            else
            {
                displayScreen.active = false;
                backViewfinderScreen.active = false;
                frontViewfinderScreen.active = false;

                CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(false);
                CameraRigManager.Instance.FollowCamera.SetDefaultTarget();
            }
        }

        private void LeftHandUpdate(Hand hand)
        {
            cameraRigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
                CameraRigManager.Instance.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
            }
        }
         
        private void RightHandUpdate(Hand hand)
        {
            cameraRigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = CameraRigManager.Instance.FOVController.fovChangeRate;

                CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
                CameraRigManager.Instance.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
            }
        }

        private void LeftHandDetached(Hand hand)
        {
            if (Settings.Camera.handheldKinematicOnRelease)
            {
                cameraRigidbody.isKinematic = true;
            }
        }

        private void RightHandDetached(Hand hand)
        {
            if (Settings.Camera.handheldKinematicOnRelease)
            {
                cameraRigidbody.isKinematic = true;
            }
        }
    }
}
