using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;

namespace NEP.MonoDirector.Patches.Guns
{
    public static class MagazinePatches
    {
        [HarmonyLib.HarmonyPatch(typeof(Magazine), nameof(Magazine.OnGrab))]
        public static class OnGrab
        {
            public static void Postfix(Hand hand)
            {
                if (Director.PlayState != State.PlayState.Recording)
                {
                    return;
                }

                HandReciever receiver = null;

                if (hand.AttachedReceiver == null)
                {
                    receiver = hand.HoveringReceiver;
                }
                else
                {
                    receiver = hand.AttachedReceiver;
                }
                
                var poolee = receiver.Host.Rb.GetComponent<MarrowEntity>();
                PropBuilder.BuildProp(poolee);

                // HACK:
                // Only show the magazine when it's being grabbed.
                // Insert two keyframes: one inactive and one active.
                var prop = poolee.GetComponent<Prop>();
                prop.RecordAction(() => prop.gameObject.SetActive(false), 0f);
                prop.RecordAction(() => prop.gameObject.SetActive(true), Recorder.Instance.RecordingTime);
            }
        }
    }
}