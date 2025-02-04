using HarmonyLib;

using Il2CppSLZ.Marrow;

using NEP.MonoDirector;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

public static class Seat
{
    [HarmonyPatch(typeof(Il2CppSLZ.Marrow.Seat))]
    [HarmonyPatch(nameof(Il2CppSLZ.Marrow.Seat.Register))]
    public static class Register
    {
        public static void Postfix(Il2CppSLZ.Marrow.Seat __instance, RigManager rM)
        {
            Main.Logger.Msg("Register Rig");
            Actor activeActor = Director.Instance.ActiveActor;

            if(activeActor == null)
            {
                return;
            }
        }
    }

    [HarmonyPatch(typeof(Il2CppSLZ.Marrow.Seat))]
    [HarmonyPatch(nameof(Il2CppSLZ.Marrow.Seat.DeRegister))]
    public static class DeRegister
    {
        public static void Postfix()
        {
            Main.Logger.Msg("Deregister Rig");

            Actor activeActor = Director.Instance.ActiveActor;

            if (activeActor == null)
            {
                return;
            }
        }
    }
}