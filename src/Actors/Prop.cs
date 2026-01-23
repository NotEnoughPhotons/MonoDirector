using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Prop(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public Trackable Actor { get => actor; }

        public List<ObjectFrame> PropFrames { get => propFrames; }
        public Rigidbody InteractableRigidbody { get => interactableRigidbody; }
        public bool isRecording;

        public static readonly Il2CppSystem.Type[] whitelistedTypes = new Il2CppSystem.Type[]
        {
            Il2CppInterop.Runtime.Il2CppType.Of<Gun>(),
            Il2CppInterop.Runtime.Il2CppType.Of<Magazine>(),
            Il2CppInterop.Runtime.Il2CppType.Of<ObjectDestructible>(),
            Il2CppInterop.Runtime.Il2CppType.Of<Atv>()
        };

        private Trackable actor;
        private Rigidbody interactableRigidbody;

        protected int stateTick;
        protected int recordedTicks;

        protected List<ObjectFrame> propFrames;
        protected List<ActionFrame> actionFrames;

        protected virtual void Awake()
        {
            propFrames = new List<ObjectFrame>();
            actionFrames = new List<ActionFrame>();
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
            interactableRigidbody = rigidbody;
        }

        public void SetActor(Trackable actor)
        {
            this.actor = actor;
        }

        public void SetPhysicsActive(bool enable)
        {
            interactableRigidbody.isKinematic = enable;
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

            if(interactableRigidbody != null)
            {
                interactableRigidbody.isKinematic = true;
            }
        }

        public virtual void Act()
        {
            gameObject.SetActive(true);

            if(interactableRigidbody == null)
            {
                interactableRigidbody = GetComponent<Rigidbody>();
            }
            else
            {
                interactableRigidbody.isKinematic = true;
            }

            transform.position = Interpolator.InterpolatePosition(PropFrames);
            transform.rotation = Interpolator.InterpolateRotation(PropFrames);

            foreach(var actionFrame in actionFrames)
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
                frameTime = Recorder.instance.RecordingTime
            };

            if (frame == 0 || interactableRigidbody != null && interactableRigidbody.IsSleeping())
            {
                propFrames.Add(objectFrame);
            }
            else
            {
                propFrames.Add(objectFrame);
                recordedTicks++;
            }
        }

        public virtual void RecordAction(Action action)
        {
            if (Director.PlayState == State.PlayState.Recording)
            {
                if (!Director.instance.RecordingProps.Contains(this))
                {
                    return;
                }

                actionFrames.Add(new ActionFrame(action, Recorder.instance.RecordingTime));
            }
        }

        public void ResetTicks()
        {
            stateTick = 0;
        }
    }
}
