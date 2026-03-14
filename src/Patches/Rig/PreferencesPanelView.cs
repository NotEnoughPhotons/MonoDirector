using UnityEngine;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;
using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Cameras;

namespace NEP.MonoDirector.Patches
{
    internal static class PreferencesPanelViewPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(PreferencesPanelView), nameof(PreferencesPanelView.OnEnable))]
        internal static class OnEnable
        {
            internal static void Postfix(RigScreenOptions __instance)
            {
                if (Director.PlayState == State.PlayState.Recording)
                {
                    Director.Recorder.SetUsedMenu(true);
                }
            }
        }
    }
}
