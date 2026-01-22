using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Patches
{
    internal static class PlayerAvatarArtPatches
    {
        [HarmonyLib.HarmonyPatch(typeof(PlayerAvatarArt), nameof(PlayerAvatarArt.UpdateAvatarHead))]
        internal static class UpdateAvatarHead
        {
            internal static Vector3 preTransformHead;
            internal static Vector3 postTransformHead;
            
            internal static Vector3 calculatedHeadOffset;
            
            internal static void Prefix(PlayerAvatarArt __instance)
            {
                Transform head = __instance._openCtrlRig.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                preTransformHead = head.position;
            }

            internal static void Postfix(PlayerAvatarArt __instance)
            {
                Transform head = __instance._openCtrlRig.avatar.animator.GetBoneTransform(HumanBodyBones.Head);
                postTransformHead = head.position;

                calculatedHeadOffset = preTransformHead - postTransformHead;
            }
        }
    }
}