using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Proxy;
using UnityEngine;

namespace NEP.MonoDirector.Actors
{
    public static class PropBuilder
    {
        public static void BuildProp(InteractableHost interactableHost)
        {
            if (interactableHost == null)
            {
                return;
            }

            var gameObject = interactableHost.gameObject;
            var rigidbody = interactableHost.Rb;

            bool hasRigidbody = rigidbody != null;
            bool isProp = gameObject.GetComponent<Prop>() != null;

            if (!hasRigidbody)
            {
                return;
            }

            if (isProp)
            {
                return;
            }

            var vfxBlip = rigidbody.GetComponent<Blip>();

            if (Prop.EligibleWithType<Gun>(rigidbody))
            {
                Logging.Msg($"Adding gun component to {gameObject.name}");

                var actorProp = gameObject.AddComponent<GunProp>();
                actorProp.SetRigidbody(rigidbody);
                actorProp.SetGun(gameObject.GetComponent<Gun>());

                vfxBlip?.CallSpawnEffect();

                Caster.AddProp(actorProp);
                return;
            }

            if (Prop.EligibleWithType<ObjectDestructible>(rigidbody))
            {
                Logging.Msg($"Adding destructable component to {gameObject.name}");

                var destructableProp = gameObject.AddComponent<BreakableProp>();
                destructableProp.SetRigidbody(rigidbody);
                destructableProp.SetBreakableObject(gameObject.GetComponent<ObjectDestructible>());

                vfxBlip?.CallSpawnEffect();

                Caster.AddRecordProp(destructableProp);
                return;
            }

            if (Prop.EligibleWithType<Magazine>(rigidbody))
            {
                Logging.Msg($"Adding magazine component to {gameObject.name}");

                var magazineProp = gameObject.AddComponent<Prop>();
                magazineProp.SetRigidbody(rigidbody);

                vfxBlip?.CallSpawnEffect();

                Caster.AddRecordProp(magazineProp);
                return;
            }

            if (Prop.EligibleWithType<Atv>(rigidbody))
            {
                Logging.Msg($"Adding vehicle component to {gameObject.name}");

                var vehicle = gameObject.AddComponent<TrackedVehicle>();
                vehicle.SetRigidbody(rigidbody);
                vehicle.SetVehicle(rigidbody.GetComponent<Atv>());

                vfxBlip?.CallSpawnEffect();

                Caster.AddRecordProp(vehicle);
                return;
            }

            if (Prop.IsActorProp(rigidbody))
            {
                Logging.Msg($"Adding prop component to {rigidbody.name}");

                var actorProp = gameObject.AddComponent<Prop>();
                actorProp.SetRigidbody(rigidbody);

                vfxBlip?.CallSpawnEffect();

                Caster.AddRecordProp(actorProp);
            }
        }

        public static void RemoveProp(InteractableHost interactableHost)
        {
            var gameObject = interactableHost.gameObject;
            var vfxBlip = gameObject.GetComponent<Blip>();

            Prop actorProp = gameObject.GetComponent<Prop>();
            bool isProp = actorProp != null;

            TrackedVehicle vehicle = actorProp.TryCast<TrackedVehicle>();
            
            if (vehicle != null)
            {
                vehicle.RemoveVehicle();
            }
            
            if (isProp && Director.PlayState == State.PlayState.Stopped)
            {
                MelonLoader.MelonLogger.Msg($"Removing component from {gameObject.name}");

                var prop = actorProp;
                vfxBlip?.CallDespawnEffect();
                Caster.RemoveProp(prop);
                GameObject.Destroy(prop);
                vfxBlip?.CallDespawnEffect();

                Events.OnPropRemoved?.Invoke(prop);
            }
        }
    }
}
