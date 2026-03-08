using NEP.MonoDirector.Data;
using NEP.MonoDirector.State;
using NEP.MonoDirector.Core;

using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class FollowCamera(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public readonly Dictionary<BodyPart, BodyPartData> FollowPoints = new Dictionary<BodyPart, BodyPartData>();

        public Transform FollowTarget { get => m_followTarget; }

        public float delta = 4f;

        private Vector3 m_positionOffset;
        private Vector3 m_rotationEulerOffset;

        private Transform m_followTarget;

        protected void Update()
        {
            if(m_followTarget == null)
            {
                return;
            }

            transform.position = m_followTarget.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, m_followTarget.rotation, delta * Time.deltaTime);
        }

        public void SetDefaultTarget()
        {
            SetFollowTarget(FollowPoints[BodyPart.Head].transform);
        }

        public void SetFollowTarget(Transform target)
        {
            m_followTarget = target;
        }

        public void SetPositionOffset(Vector3 offset)
        {
            m_positionOffset = offset;
        }

        public void SetRotationOffset(Vector3 offset)
        {
            m_rotationEulerOffset = offset;
        }

        public void SetRotationOffset(Quaternion offset)
        {
            m_rotationEulerOffset = offset.eulerAngles;
        }

        public void SetFollowBone(BodyPart part)
        {
            m_positionOffset = Vector3.zero;
            m_rotationEulerOffset = Vector3.zero;

            Vector3 point = FollowPoints[part].position;

            m_followTarget.position = point;

            m_followTarget.localPosition = m_positionOffset;
            m_followTarget.localRotation = Quaternion.Euler(m_rotationEulerOffset);
        }
    }
}
