using HarmonyLib;
using NEP.MonoDirector;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

using Il2CppSLZ.Marrow;

public static class SeatPatches
{
    [HarmonyPatch(typeof(Seat))]
    [HarmonyPatch(nameof(Seat.Register))]
    public static class Register
    {
        public static void Postfix(Seat __instance, RigManager rM)
        {
            Main.Logger.Msg("Register Rig");
            Actor activeActor = Recorder.instance.ActiveActor;

            if(activeActor == null)
            {
                return;
            }
            
            activeActor.RecordAction(() => activeActor.ParentToSeat(__instance));
        }
    }

    [HarmonyPatch(typeof(Seat))]
    [HarmonyPatch(nameof(Seat.DeRegister))]
    public static class DeRegister
    {
        public static void Postfix()
        {
            Main.Logger.Msg("Deregister Rig");

            Actor activeActor = Recorder.instance.ActiveActor;

            if (activeActor == null)
            {
                return;
            }

            activeActor.RecordAction(() => activeActor.UnparentSeat());
        }
    }
}