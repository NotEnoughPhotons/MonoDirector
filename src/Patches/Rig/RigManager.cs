using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using Avatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Patches
{
    internal static class RigManager
    {
        [HarmonyLib.HarmonyPatch(typeof(Il2CppSLZ.Marrow.RigManager), nameof(Il2CppSLZ.Marrow.RigManager.SwitchAvatar))]
        internal static class SwitchAvatar
        {
            internal static void Postfix(Avatar newAvatar)
            {
                // TODO:
                // See about multiple actors with new control flow
                // This will remain broken until I figure out a way
                // to pass a list around with this new system.

                if(Director.Instance.InRecordingMode)
                {
                    return;
                }

                // Actor activeActor = Director.Instance.ActiveActor;
                // activeActor.RecordAction(new System.Action(() => activeActor.SwitchToActor(activeActor)));
                // activeActor.CloneAvatar();
                // Director.Instance.ActiveActors.Add(activeActor);
                // Director.Instance.StageActor(newAvatar);
            }
        }
    }
}
