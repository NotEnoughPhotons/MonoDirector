using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

using Il2CppMK.Glow.URP;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class CameraVolume : MonoBehaviour
    {
        public CameraVolume(System.IntPtr ptr) : base(ptr) { }

        public Volume RenderingVolume { get; private set; }

        public LensDistortion LensDistortion { get; private set; }
        public ChromaticAberration ChromaticAberration { get; private set; }
        public MKGlow MkGlow { get; private set; }
        public Vignette Vignette { get; private set; }
        public Bloom Bloom { get; private set; }

        private void Start()
        {
            RenderingVolume = GetComponent<Volume>();

            LensDistortion = RenderingVolume.profile.components[0].Cast<LensDistortion>();
            ChromaticAberration = RenderingVolume.profile.components[1].Cast<ChromaticAberration>();
            MkGlow = RenderingVolume.profile.components[4].Cast<MKGlow>();
        }

        public void EnableAll(bool enabled)
        {
            foreach(VolumeComponent component in RenderingVolume.profile.components)
            {
                component.SetAllOverridesTo(enabled);
            }
        }

        public void SetValue(FloatParameter parameter, float value)
        {
            if(parameter == null)
            {
                return;
            }

            parameter.value = value;
        }
    }
}
