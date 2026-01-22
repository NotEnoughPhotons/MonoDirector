using NEP.MonoDirector.Core;

using UnityEngine;

using Il2CppSLZ.Marrow;
using Il2CppSLZ.Bonelab;

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
                Main.Logger.Msg($"Adding gun component to {gameObject.name}");

                var actorProp = gameObject.AddComponent<GunProp>();
                actorProp.SetRigidbody(rigidbody);
                actorProp.SetGun(gameObject.GetComponent<Gun>());
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(actorProp);
                return;
            }

            if (Prop.EligibleWithType<ObjectDestructible>(rigidbody))
            {
                Main.Logger.Msg($"Adding destructable component to {gameObject.name}");

                var destructableProp = gameObject.AddComponent<BreakableProp>();
                destructableProp.SetRigidbody(rigidbody);
                destructableProp.SetBreakableObject(gameObject.GetComponent<ObjectDestructible>());

                Director.instance.RecordingProps.Add(destructableProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(destructableProp);
                return;
            }

            if (Prop.EligibleWithType<Magazine>(rigidbody))
            {
                Main.Logger.Msg($"Adding magazine component to {gameObject.name}");

                var magazineProp = gameObject.AddComponent<Prop>();
                magazineProp.SetRigidbody(rigidbody);

                Director.instance.RecordingProps.Add(magazineProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(magazineProp);
                return;
            }

            if (Prop.EligibleWithType<Atv>(rigidbody))
            {
                Main.Logger.Msg($"Adding vehicle component to {gameObject.name}");

                var vehicle = gameObject.AddComponent<TrackedVehicle>();
                vehicle.SetRigidbody(rigidbody);
                vehicle.SetVehicle(rigidbody.GetComponent<Atv>());

                Director.instance.RecordingProps.Add(vehicle);
                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(vehicle);
                return;
            }

            if (Prop.IsActorProp(rigidbody))
            {
                Main.Logger.Msg($"Adding prop component to {rigidbody.name}");

                var actorProp = gameObject.AddComponent<Prop>();
                actorProp.SetRigidbody(rigidbody);
                Director.instance.RecordingProps.Add(actorProp);

                vfxBlip?.CallSpawnEffect();

                Events.OnPropCreated?.Invoke(actorProp);
            }
        }

        public static void RemoveProp(InteractableHost interactableHost)
        {
            var gameObject = interactableHost.gameObject;
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

                Events.OnPropRemoved?.Invoke(prop);
            }
        }
    }
}
