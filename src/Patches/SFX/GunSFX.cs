using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches
{
    internal static class GunSFXPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.MagazineInsert))]
        internal static class MagazineInsert
        {
            internal static void Postfix(GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineInsert);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.MagazineDrop))]
        internal static class MagazineDrop
        {
            internal static void Postfix(GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.MagazineDrop);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlidePull))]
        internal static class SlidePull
        {
            internal static void Postfix(GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlidePull);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlideRelease))]
        internal static class SlideRelease
        {
            internal static void Postfix(GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideRelease);
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(GunSFX), nameof(GunSFX.SlideLock))]
        internal static class SlideLock
        {
            internal static void Postfix(GunSFX __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(gunProp.Gun.gunSFX.SlideLock);
            }
        }
    }
}
