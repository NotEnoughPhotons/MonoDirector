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
        public static void BuildProp(MarrowEntity entity)
        {
            if (!entity)
                return;

            if (entity.Bodies.Count == 0)
                return;

            if (entity.GetComponent<Prop>())
                return;

            if (Prop.EligibleWithType<Gun>(entity))
            {
#if DEBUG
                Logging.Msg($"Adding gun component to {entity.name}");
#endif
                GunProp prop = entity.gameObject.AddComponent<GunProp>();
                prop.SetEntity(entity);
                prop.SetGun(entity.GetComponent<Gun>());

                Caster.AddRecordProp(prop);
                return;
            }

            if (Prop.EligibleWithType<ObjectDestructible>(entity))
            {
#if DEBUG
                Logging.Msg($"Adding destructable component to {entity.name}");
#endif
                BreakableProp prop = entity.gameObject.AddComponent<BreakableProp>();
                prop.SetEntity(entity);
                prop.SetBreakableObject(entity.GetComponent<ObjectDestructible>());

                Caster.AddRecordProp(prop);
                return;
            }

            if (Prop.EligibleWithType<Magazine>(entity))
            {
#if DEBUG
                Logging.Msg($"Adding magazine component to {entity.name}");
#endif
                Prop prop = entity.gameObject.AddComponent<Prop>();
                prop.SetEntity(entity);

                Caster.AddRecordProp(prop);
                return;
            }

            if (Prop.EligibleWithType<Atv>(entity))
            {
#if DEBUG
                Logging.Msg($"Adding vehicle component to {entity.name}");
#endif
                Prop prop = entity.gameObject.AddComponent<Prop>();
                prop.SetEntity(entity);
                //prop.SetVehicle(entity.GetComponent<Atv>());

                Caster.AddRecordProp(prop);
                return;
            }

            if (Prop.IsActorProp(entity))
            {
#if DEBUG
                Logging.Msg($"Adding prop component to {entity.name}");
#endif
                Prop prop = entity.gameObject.AddComponent<Prop>();
                prop.SetEntity(entity);

                Caster.AddRecordProp(prop);
            }
        }
        
        public static void RemoveProp(MarrowEntity entity)
        {
            Prop actorProp = entity.GetComponent<Prop>();

            if (Director.PlayState == State.PlayState.Stopped)
            {
#if DEBUG
                MelonLoader.MelonLogger.Msg($"Removing component from {entity.name}");
#endif
                if (actorProp is TrackedVehicle vehicle)
                {
                    vehicle.RemoveVehicle();
                }

                Caster.RemoveProp(actorProp);
                Director.ActiveStage.RemoveProp(actorProp);
                GameObject.Destroy(actorProp);
            }
        } 
    }
}
