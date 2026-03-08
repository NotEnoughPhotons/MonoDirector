using UnityEngine;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;
using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Cameras;

namespace NEP.MonoDirector.Patches
{
    internal static class RigScreenOptionsPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(RigScreenOptions), nameof(RigScreenOptions.Start))]
        internal static class Start
        {
            internal static void Postfix(RigScreenOptions __instance)
            {
                new CameraRigManager(__instance);
            }
        }
    }
}
