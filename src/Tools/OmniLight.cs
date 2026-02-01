using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class OmniLight(IntPtr ptr) : PointToolEntity(ptr)
    {
        public static List<OmniLight> ComponentCache { get; private set; }

        public float Range { get; private set; }
        public float Intensity { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            ComponentCache = new List<OmniLight>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            ComponentCache.Add(this);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ComponentCache.Remove(this);
        }
    }
}