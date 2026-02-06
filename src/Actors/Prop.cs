using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Prop(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Trackable Actor { get => m_actor; }

        public List<ObjectFrame> PropFrames { get => m_propFrames; }
        public Rigidbody InteractableRigidbody { get => m_interactableRigidbody; }
        public bool isRecording;

        public static readonly Il2CppSystem.Type[] whitelistedTypes = new Il2CppSystem.Type[]
        {
            Il2CppInterop.Runtime.Il2CppType.Of<Gun>(),
            Il2CppInterop.Runtime.Il2CppType.Of<Magazine>(),
            Il2CppInterop.Runtime.Il2CppType.Of<ObjectDestructible>(),
            Il2CppInterop.Runtime.Il2CppType.Of<Atv>()
        };

        private Trackable m_actor;
        private Rigidbody m_interactableRigidbody;

        protected int m_stateTick;
        protected int m_recordedTicks;

        protected List<ObjectFrame> m_propFrames;
        protected List<ActionFrame> m_actionFrames;

        protected virtual void Awake()
        {
            m_propFrames = new List<ObjectFrame>();
            m_actionFrames = new List<ActionFrame>();
        }

        public static bool IsActorProp(Rigidbody rigidbody)
        {
            if (rigidbody == null)
            {
                return false;
            }

            if (rigidbody.isKinematic || rigidbody.gameObject.isStatic)
            {
                return false;
            }

            if (rigidbody.gameObject.layer == LayerMask.NameToLayer("EnemyColliders"))
            {
                return false;
            }

            if(rigidbody.GetComponent<InteractableHost>() == null)
            {
                return false;
            }

            if (rigidbody.GetComponent<Prop>() != null || rigidbody.GetComponent<WorldGrip>() != null)
            {
                return false;
            }

            return true;
        }

        public static bool EligibleWithType<T>(Rigidbody rigidbody)
        {
            return rigidbody.GetComponent<T>() != null;
        }

        public void SetRigidbody(Rigidbody rigidbody)
        {
            m_interactableRigidbody = rigidbody;
        }

        public void SetActor(Trackable actor)
        {
            this.m_actor = actor;
        }

        public void SetPhysicsActive(bool enable)
        {
            m_interactableRigidbody.isKinematic = enable;
        }

        public virtual void OnSceneBegin()
        {
            if(PropFrames == null)
            {
                return;
            }

            if(PropFrames.Count == 0)
            {
                return;
            }

            transform.position = PropFrames[0].position;
            transform.rotation = PropFrames[0].rotation;
            transform.localScale = PropFrames[0].scale;

            if(m_interactableRigidbody != null)
            {
                m_interactableRigidbody.isKinematic = true;
            }
        }

        public virtual void Act()
        {
            gameObject.SetActive(true);

            if(m_interactableRigidbody == null)
            {
                m_interactableRigidbody = GetComponent<Rigidbody>();
            }
            else
            {
                m_interactableRigidbody.isKinematic = true;
            }

            transform.position = Interpolator.InterpolatePosition(PropFrames);
            transform.rotation = Interpolator.InterpolateRotation(PropFrames);

            foreach(var actionFrame in m_actionFrames)
            {
                if (Playback.Instance.PlaybackTime < actionFrame.timestamp)
                {
                    continue;
                }
                else
                {
                    actionFrame.Run();
                }
            }
        }

        public virtual void Record(int frame)
        {
            isRecording = true;

            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform,
                position = transform.position,
                rotation = transform.rotation,
                scale = transform.localScale,
                frameTime = Recorder.Instance.RecordingTime
            };

            if (frame == 0 || m_interactableRigidbody != null && m_interactableRigidbody.IsSleeping())
            {
                m_propFrames.Add(objectFrame);
            }
            else
            {
                m_propFrames.Add(objectFrame);
                m_recordedTicks++;
            }
        }

        public virtual void RecordAction(Action action)
        {
            if (Director.PlayState == State.PlayState.Recording)
            {
                if (!Director.RecordingProps.Contains(this))
                {
                    return;
                }

                m_actionFrames.Add(new ActionFrame(action, Recorder.Instance.RecordingTime));
            }
        }

        public void ResetTicks()
        {
            m_stateTick = 0;
        }
    }
}
