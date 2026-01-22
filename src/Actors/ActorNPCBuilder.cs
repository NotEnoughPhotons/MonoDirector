using static MelonLoader.MelonLogger;

using NEP.MonoDirector.Core;

using UnityEngine;

using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.VFX;

namespace NEP.MonoDirector.Actors
{
    public static class ActorNPCBuilder
    {
        public static void BuildNPCActor(Poolee pooleeObject)
        {
            ActorNPC test = new ActorNPC(pooleeObject.transform);
            Director.instance.NPCCast.Add(test);
        }

        public static void RemoveNPCActor(Poolee pooleeObject)
        {
            var gameObject = pooleeObject.gameObject;
            var vfxBlip = gameObject.GetComponent<Blip>();

            Prop actorProp = gameObject.GetComponent<Prop>();
            bool isProp = actorProp != null;

            if (isProp && Director.PlayState == State.PlayState.Stopped)
            {
                MelonLoader.MelonLogger.Msg($"Removing component from {gameObject.name}");

                var prop = actorProp;
                prop.InteractableRigidbody.isKinematic = false;
                Director.instance.RecordingProps.Remove(prop);
                GameObject.Destroy(prop);
                vfxBlip?.CallDespawnEffect();
            }
        }
    }
}
