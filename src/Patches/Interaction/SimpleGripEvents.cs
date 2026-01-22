using NEP.MonoDirector.Actors;
using UnityEngine;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches
{
    internal static class SimpleGripEventsPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(SimpleGripEvents), nameof(SimpleGripEvents.OnAttachedDelegate))]
        internal static class OnAttachedDelegate
        {
            internal static void Postfix(SimpleGripEvents __instance, Hand hand)
            {
               if(__instance.GetComponent<GripEventListener>() == null)
               {
                    var listener = __instance.gameObject.AddComponent<GripEventListener>();
                    listener.SetProp(__instance.Grips[0].Host.Rb.GetComponent<Prop>());
               }
            }
        }

        [HarmonyLib.HarmonyPatch(typeof(SimpleGripEvents), nameof(SimpleGripEvents.OnDetachedDelegate))]
        internal static class OnDetachedDelegate
        {
            internal static void Postfix(SimpleGripEvents __instance, Hand hand)
            {
                GripEventListener listener = __instance.GetComponent<GripEventListener>();

                if (listener != null)
                {
                    GameObject.Destroy(listener);
                }
            }
        }
    }
}
