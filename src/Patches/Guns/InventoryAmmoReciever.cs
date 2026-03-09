using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches.Guns
{
    public static class MagazinePatches
    {
        [HarmonyLib.HarmonyPatch(typeof(Magazine), nameof(Magazine.OnGrab))]
        public static class OnGrab
        {
            public static void Postfix(Hand hand)
            {
                if (Director.PlayState == State.PlayState.Recording)
                {
                    HandReciever reciever = hand.AttachedReceiver;
                    var poolee = reciever.Host.Rb.GetComponent<InteractableHost>();
                    PropBuilder.BuildProp(poolee);

                    // HACK:
                    // Only show the magazine when it's being grabbed.
                    // Insert two keyframes: one inactive and one active.
                    if (Director.PlayState == State.PlayState.Recording)
                    {
                        var prop = poolee.GetComponent<Prop>();
                        prop.RecordAction(() => prop.gameObject.SetActive(false), 0f);
                        prop.RecordAction(() => prop.gameObject.SetActive(true), Recorder.Instance.RecordingTime);
                    }
                }
            }
        }
    }
}