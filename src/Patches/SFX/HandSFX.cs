using UnityEngine;

using NEP.MonoDirector.Audio;

using Il2CppSLZ.Marrow;

using Random = UnityEngine.Random;

namespace NEP.MonoDirector.Patches
{
    internal static class HandSFXPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(HandSFX), nameof(HandSFX.Grab))]
        internal static class Grab
        {
            internal static void Postfix(HandSFX __instance)
            {
                /* var activeActor = Recorder.instance.ActiveActor;

                if (activeActor == null)
                {
                    return;
                }

                if(Director.PlayState == State.PlayState.Recording)
                {
                    activeActor.RecordAction(() => PlaySFX(__instance, __instance.transform.position));
                } */
            }

            internal static void PlaySFX(HandSFX hand, Vector3 position)
            {
                int rand = Random.Range(0, hand.grab.Count);

                // AudioManager.Instance.PlayAtPosition(hand.grab[rand], position);
            }
        }
    }
}
