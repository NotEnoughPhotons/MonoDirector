using NEP.MonoDirector.State;

using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class HandheldCamera(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private CylinderGrip m_leftHandle;
        private CylinderGrip m_rightHandle;

        private Transform m_leftHandleTransform;
        private Transform m_rightHandleTransform;

        private Camera m_camera;
        private Camera m_screenCamera;

        private GameObject m_frontScreen;
        private GameObject m_displayScreen;

        private Rigidbody m_rigidbody;

        private Vector3 m_lastPosition;

        private RenderTexture m_displayTexture  => m_screenCamera.targetTexture;

        private void Awake()
        {
            m_leftHandleTransform = transform.Find("Grips/Left Handle");
            m_rightHandleTransform = transform.Find("Grips/Right Handle");

            m_camera = transform.Find("Camera").GetComponent<Camera>();
            m_screenCamera = m_camera.transform.GetChild(0).GetComponent<Camera>();
            m_frontScreen = transform.Find("ExternalScreen").gameObject;
            m_displayScreen = transform.Find("Screen").gameObject;

            m_leftHandle = m_leftHandleTransform.GetComponent<CylinderGrip>();
            m_rightHandle = m_rightHandleTransform.GetComponent<CylinderGrip>();

            m_rigidbody = GetComponent<Rigidbody>();

            m_camera.backgroundColor = Color.black;
            m_screenCamera.backgroundColor = Color.black;
            m_camera.clearFlags = CameraClearFlags.SolidColor;
            m_screenCamera.clearFlags = CameraClearFlags.SolidColor;
        }

        private void OnEnable()
        {
            OnCameraModeChanged(CameraMode.Handheld);

            m_leftHandle.attachedUpdateDelegate += new System.Action<Hand>(LeftHandUpdate);
            m_rightHandle.attachedUpdateDelegate += new System.Action<Hand>(RightHandUpdate);
            m_leftHandle.detachedHandDelegate += new System.Action<Hand>(LeftHandDetached);
            m_leftHandle.detachedHandDelegate += new System.Action<Hand>(RightHandDetached);

            // m_lastCameraRotation = camera.transform.rotation;
        }

        private void OnDisable()
        {
            OnCameraModeChanged(CameraMode.Head);

            m_leftHandle.attachedUpdateDelegate -= new System.Action<Hand>(LeftHandUpdate);
            m_rightHandle.attachedUpdateDelegate -= new System.Action<Hand>(RightHandUpdate);
            m_leftHandle.detachedHandDelegate -= new System.Action<Hand>(LeftHandDetached);
            m_leftHandle.detachedHandDelegate -= new System.Action<Hand>(RightHandDetached);
        }

        private void Update()
        {
            m_camera.transform.localPosition = Vector3.Lerp(m_lastPosition, m_camera.transform.localPosition, 8f * Time.deltaTime);
            m_screenCamera.transform.localPosition = Vector3.Lerp(m_lastPosition, m_camera.transform.localPosition, 8f * Time.deltaTime);
        }

        private void LateUpdate()
        {
            m_lastPosition = m_camera.transform.localPosition;
        }

        private void OnCameraModeChanged(CameraMode mode)
        {
            if(mode == CameraMode.Handheld)
            {
                m_displayScreen.active = true;
                m_frontScreen.active = true;

                // camera.targetTexture = displayTexture;
                m_camera.gameObject.SetActive(true);
                // CameraRigManager.Instance.FollowCamera.SetFollowTarget(sensorCamera.transform);
                // CameraRigManager.Instance.CameraDisplay.FollowCamera.SetFollowTarget(sensorCamera.transform);
            }
            else
            {
                m_displayScreen.active = false;
                m_frontScreen.active = false;

                m_camera.gameObject.SetActive(false);

                // CameraRigManager.Instance.ClonedCamera.gameObject.SetActive(false);
                // CameraRigManager.Instance.FollowCamera.SetDefaultTarget();
            }
        }

        private void LeftHandUpdate(Hand hand)
        {
            m_rigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = 4f;

                m_camera.fieldOfView += -(hand.GetIndexTriggerAxis() * rate / 10f);
                m_screenCamera.fieldOfView += -(hand.GetIndexTriggerAxis() * rate / 10f);

                // CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
                // CameraRigManager.Instance.FOVController.SetFOV(-(hand.GetIndexTriggerAxis() * rate / 10f));
            }
        }
         
        private void RightHandUpdate(Hand hand)
        {
            m_rigidbody.isKinematic = false;

            if (hand.GetIndexTriggerAxis() > 0.25f)
            {
                float rate = 4f;

                m_camera.fieldOfView += (hand.GetIndexTriggerAxis() * rate / 10f);
                m_screenCamera.fieldOfView += (hand.GetIndexTriggerAxis() * rate / 10f);

                // CameraRigManager.Instance.CameraDisplay.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
                // CameraRigManager.Instance.FOVController.SetFOV(hand.GetIndexTriggerAxis() * rate / 10f);
            }
        }

        private void LeftHandDetached(Hand hand)
        {
            if (Settings.Camera.handheldKinematicOnRelease)
            {
                m_rigidbody.isKinematic = true;
            }
        }

        private void RightHandDetached(Hand hand)
        {
            if (Settings.Camera.handheldKinematicOnRelease)
            {
                m_rigidbody.isKinematic = true;
            }
        }
    }
}
