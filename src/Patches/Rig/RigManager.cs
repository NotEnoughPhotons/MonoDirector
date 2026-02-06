using UnityEngine;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Patches
{
    internal static class RigManagerPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(RigManager), nameof(RigManager.SwitchAvatar))]
        internal static class SwitchAvatar
        {
            internal static void Postfix(MarrowAvatar newAvatar)
            {
                if(Director.PlayState != State.PlayState.Recording)
                {
                    return;
                }

                var activeActor = Recorder.Instance.ActiveActor;
                activeActor.RecordAction(new System.Action(() => activeActor.SwitchToActor(activeActor)));
                activeActor.CloneAvatar();
                Recorder.Instance.ActiveActors.Add(activeActor);
                Recorder.Instance.SetActor(newAvatar);
            }
        }
    }
}
