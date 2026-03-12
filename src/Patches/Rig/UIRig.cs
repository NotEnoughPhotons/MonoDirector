using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Rig;
using NEP.MonoDirector.UI;

namespace NEP.MonoDirector.Patches
{
    [HarmonyLib.HarmonyPatch(typeof(UIRig), nameof(UIRig.Awake))]
    internal static class UIRigPatches
    {
        public static void Postfix(UIRig __instance)
        {
            MenuBootstrap.Initialize(__instance);
        }
    }
}
