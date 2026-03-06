using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Actors;
using UnityEngine;

namespace NEP.MonoDirector.Core
{
    public static class Caster
    {
        public static event Action<Actor> OnActorCasted;
        public static event Action<Actor> OnActorRemoved;
        public static event Action<Actor> OnActorRecasted;
        public static event Action<Actor> OnActorSelected;
        public static event Action<Actor> OnActorDeselected;

        public static event Action<Prop> OnPropAdded;
        public static event Action<Prop> OnPropRemoved;

        public static Actor SelectedActor { get => m_selectedActor; }

        public static IReadOnlyList<Actor> Cast { get => m_cast.AsReadOnly(); }
        public static IReadOnlyList<Prop> Props { get => m_props.AsReadOnly(); }

        private static Actor m_selectedActor;

        private static List<Actor> m_cast;
        private static List<Prop> m_props;

        internal static void Initialize()
        {
            m_cast = new List<Actor>();
            m_props = new List<Prop>();
        }

        public static void CastActor(Actor actor)
        {
            m_cast.Add(actor);
            OnActorCasted?.Invoke(actor);
        }

        public static void UncastActor(Actor actor)
        {
            m_cast.Remove(actor);
            OnActorRemoved?.Invoke(actor);
        }

        public static void RecastActor(Actor actor)
        {
            Vector3 actorPosition = actor.Frames[0].TransformFrames[0].position;
            Constants.RigManager.Teleport(actorPosition, true);
            Constants.RigManager.SwapAvatar(actor.ClonedAvatar);

            // Any props recorded by this actor must be removed if we're recasting
            // If we don't, the props will still play, but they will be floating in the air aimlessly.
            // Spooky!

            if (m_props.Count != 0)
            {
                foreach (var prop in m_props)
                {
                    if (prop.Actor == actor)
                    {
                        GameObject.Destroy(prop);
                    }
                }
            }

            UncastActor(actor);
            actor.Delete();

            OnActorRecasted?.Invoke(actor);
        }

        public static void SelectActor(Actor actor)
        {
            m_selectedActor = actor;
            OnActorSelected?.Invoke(actor);
        }

        public static void DeselectActor(Actor actor)
        {
            m_selectedActor = null;
            OnActorDeselected?.Invoke(actor);
        }

        public static void AddProp(Prop prop)
        {
            m_props.Add(prop);
            OnPropAdded?.Invoke(prop);
        }

        public static void RemoveProp(Prop prop)
        {
            m_props.Remove(prop);
            OnPropRemoved?.Invoke(prop);
        }
    }
}
