using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SpotLight(IntPtr ptr) : PointToolEntity(ptr)
    {
        public static List<SpotLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        private Rigidbody rb;

        private GameObject sprite;

        private Grip lightGrip;

        private GameObject arrow;

        protected override void Awake()
        {
            base.Awake();
            ComponentCache = new List<SpotLight>();
        }
    }
}