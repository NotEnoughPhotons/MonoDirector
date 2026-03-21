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

        public static Actor SelectedActor => m_selectedActor;

        public static IReadOnlyList<Actor> Cast => m_cast.AsReadOnly();
        public static IReadOnlyList<Prop> Props => m_props.AsReadOnly();
        public static IReadOnlyList<Prop> RecordProps => m_recordProps.AsReadOnly();

        private static Actor m_selectedActor;

        private static List<Actor> m_cast;
        private static List<Prop> m_props;
        private static List<Prop> m_recordProps;

        internal static void Initialize()
        {
            m_cast = new List<Actor>();
            m_props = new List<Prop>();
            m_recordProps = new List<Prop>();
        }

        public static void CastActor(Actor actor)
        {
            m_cast.Add(actor);
            OnActorCasted?.Invoke(actor);
        }

        public static void CastActors(List<Actor> actors)
        {
            foreach (var actor in actors)
            {
                CastActor(actor);
            }
        }

        public static void UncastActor(Actor actor)
        {
            actor.Delete();
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

            if (actor.OwnedProps.Count != 0)
            {
                foreach (var prop in actor.OwnedProps)
                {
                    GameObject.Destroy(prop);
                }
            }

            UncastActor(actor);

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

        public static void AddProps(List<Prop> props)
        {
            foreach (var prop in props)
            {
                AddProp(prop);
            }
        }

        public static void RemoveProp(Prop prop)
        {
            prop.SetPhysicsActive(true);
            m_props.Remove(prop);
            m_recordProps.Remove(prop);
            OnPropRemoved?.Invoke(prop);
        }

        public static void AddRecordProp(Prop prop)
        {
            m_recordProps.Add(prop);
            OnPropAdded?.Invoke(prop);
        }

        public static void RemoveRecordProp(Prop prop)
        {
            m_recordProps.Remove(prop);
            OnPropRemoved?.Invoke(prop);
        }

        public static void TransferRecordedProps()
        {
            m_props.AddRange(m_recordProps);
            ClearRecordProps();
        }

        public static void ClearCast()
        {
            m_cast.Clear();

            if (m_selectedActor != null)
            {
                DeselectActor(m_selectedActor);
            }
        }

        public static void ClearProps()
        {
            m_props.Clear();
            m_recordProps.Clear();
        }

        public static void ClearRecordProps()
        {
            m_recordProps.Clear();
        }
    }
}
