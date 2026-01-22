using NEP.MonoDirector.Actors;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Data;

namespace NEP.MonoDirector.Patches
{
    internal static class GunPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnFire))]
        internal static class OnFire
        {
            internal static void Postfix(Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => gunProp.GunFakeFire()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.SetAnimationState))]
        internal static class PlayAnimationState
        {
            internal static void Postfix(Gun __instance, Gun.AnimationStates state, float perc)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(() => __instance.SetAnimationState(state, perc));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineInserted))]
        internal static class OnMagazineInserted
        {
            internal static void Postfix(Gun __instance)
            {
                if (__instance._magState != null)
                {
                    var gunProp = __instance.gameObject.GetComponent<GunProp>();
                    int count = __instance._magState.AmmoCount;
                    CartridgeData cartridgeData = __instance._magState.cartridgeData;
                    MagazineData magazineData = __instance._magState.magazineData;
                    gunProp?.RecordAction(new System.Action(() => gunProp.InsertMagState(cartridgeData, magazineData, count)));
                }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.OnMagazineRemoved))]
        internal static class OnMagazineRemoved
        {
            internal static void Postfix(Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => gunProp.RemoveMagState()));
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(Gun), nameof(Gun.UpdateArt))]
        internal static class UpdateArt
        {
            internal static void Postfix(Gun __instance)
            {
                var gunProp = __instance.gameObject.GetComponent<GunProp>();
                gunProp?.RecordAction(new System.Action(() => __instance.UpdateArt()));
            }
        }
    }
}
