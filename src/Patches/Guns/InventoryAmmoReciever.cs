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
                }
            }
        }
    }
}