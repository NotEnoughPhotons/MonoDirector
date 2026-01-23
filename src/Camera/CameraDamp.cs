using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraDamp(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public float delta = 4f;

        private Quaternion lastRotation;

        private void LateUpdate()
        {
            //lastRotation = transform.rotation;
            //transform.rotation = Quaternion.Slerp(lastRotation, transform.rotation, delta * Time.deltaTime);
        }
    }
}
