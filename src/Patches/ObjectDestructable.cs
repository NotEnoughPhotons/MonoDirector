using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches
{
    internal static class ObjectDestructiblePatches
    {
        [HarmonyLib.HarmonyPatch(typeof(ObjectDestructible), nameof(ObjectDestructible.Awake))]
        internal static class TakeDamage
        {
            static void Postfix(ObjectDestructible __instance)
            {
                __instance.OnDestruction += new System.Action<ObjectDestructible>(OnObjectDestroyed);
            }

            static void OnObjectDestroyed(ObjectDestructible destructable)
            {
                var prop = destructable.GetComponent<BreakableProp>();
                prop?.RecordAction(prop.DestructionEvent);
            }
        }
    }
}
