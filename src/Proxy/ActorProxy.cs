using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Core;
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
            triggerHullObject.transform.localPosition = Vector3.up;
            triggerHullObject.transform.localRotation = Quaternion.identity;
            triggerHullObject.SetActive(false);
        }

        public void SetActor(Actor actor)
        {
            m_actor = actor;

            m_triggerHull.size = new Vector3(0.5f, 1f, 0.5f);
            m_triggerHull.transform.SetParent(m_actor.ClonedAvatar.animator.GetBoneTransform(HumanBodyBones.Hips));

            if (m_frame == null)
            {
                m_frame = ActorFrameManager.AddFrameToActor(this);
            }
            else
            {
                m_frame.transform.localScale = m_triggerHull.size;
            }
        }

        public void OnSelected()
        {
            FeedbackSFX.LinkAudio();
            m_triggerHull.gameObject.SetActive(true);
            Director.SelectActor(m_actor);
        }

        public void OnDeselected()
        {
            m_triggerHull.gameObject.SetActive(false);
            Director.DeselectActor(m_actor);
        }
    }
}
