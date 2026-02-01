using NEP.MonoDirector.State;
using UnityEngine;

using Il2CppSLZ.Marrow;
using NEP.MonoDirector.Audio;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SoundSource3D(IntPtr ptr) : SoundSource(ptr)
    {
        protected override void Awake()
        {
            base.Awake();
            m_source.spatialBlend = 1f;
        }
    }
}