using UnityEngine;
namespace NEP.MonoDirector.Data
{
    public struct BodyPartData
    {
        public BodyPartData(Transform transform)
        {
            this.transform = transform;
            position = transform.position;
            rotation = transform.rotation;
        }

        public Transform transform;
        public Vector3 position;
        public Quaternion rotation;
    }
}
