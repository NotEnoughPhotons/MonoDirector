using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.UI;
using UnityEngine;
using UnityEngine.Playables;

namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorProxy(IntPtr ptr) : TrackableProxy(ptr)
    {
        public Actor Actor { get => m_actor; }
        public BoxCollider Collider { get => m_triggerHull; }

        private Actor m_actor;
        private BoxCollider m_triggerHull;
        private GameObject m_frame;

        private void Awake()
        {
            BuildHull();
        }

        private void BuildHull()
        {
            GameObject triggerHullObject = new GameObject("Actor Trigger Hull");
            m_triggerHull = triggerHullObject.AddComponent<BoxCollider>();
            m_triggerHull.isTrigger = true;
            triggerHullObject.transform.SetParent(transform);
            triggerHullObject.transform.localPosition = Vector3.zero;
            triggerHullObject.transform.localRotation = Quaternion.identity;
        }

        public void SetActor(Actor actor)
        {
            m_actor = actor;

            float avatarHeight = m_actor.ClonedAvatar.height;
            float avatarWidth = m_actor.ClonedAvatar._waistEllipseX + m_actor.ClonedAvatar._waistEllipseZ;
            m_triggerHull.size = new Vector3(avatarWidth, 1f, avatarWidth);

            if (m_frame == null)
            {
                m_frame = ActorFrameManager.AddFrameToActor(this);
            }
            else
            {
                m_frame.transform.localScale = m_triggerHull.size;
            }
        }
    }
}
