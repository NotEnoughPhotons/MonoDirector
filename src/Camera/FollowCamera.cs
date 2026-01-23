using NEP.MonoDirector.Data;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FollowCamera(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public readonly Dictionary<BodyPart, BodyPartData> FollowPoints = new Dictionary<BodyPart, BodyPartData>()
        {
            { BodyPart.Head, new BodyPartData(CameraRigManager.Instance.RigScreenOptions.TargetTransform) },
            { BodyPart.Chest, new BodyPartData(Constants.RigManager.physicsRig.m_chest) },
            { BodyPart.Pelvis, new BodyPartData(Constants.RigManager.physicsRig.m_pelvis) }
        };

        public Transform FollowTarget { get => followTarget; }

        public float delta = 4f;

        private Vector3 positionOffset;
        private Vector3 rotationEulerOffset;

        private Transform followTarget;

        protected void Update()
        {
            if(followTarget == null)
            {
                Main.Logger.Msg("Follow target is null!");
                return;
            }

            transform.position = followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, followTarget.rotation, delta * Time.deltaTime);
        }

        public void SetDefaultTarget()
        {
            SetFollowTarget(FollowPoints[BodyPart.Head].transform);
        }

        public void SetFollowTarget(Transform target)
        {
            followTarget = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            positionOffset = offset;
        }

        public void SetRotationOffset(Vector3 offset)
        {
            rotationEulerOffset = offset;
        }

        public void SetRotationOffset(Quaternion offset)
        {
            rotationEulerOffset = offset.eulerAngles;
        }

        public void SetFollowBone(BodyPart part)
        {
            positionOffset = Vector3.zero;
            rotationEulerOffset = Vector3.zero;

            Vector3 point = FollowPoints[part].position;

            followTarget.position = point;

            followTarget.localPosition = positionOffset;
            followTarget.localRotation = Quaternion.Euler(rotationEulerOffset);
        }
    }
}
