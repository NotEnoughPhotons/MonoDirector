using UnityEngine;

using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class BreakableProp(IntPtr ptr) : Prop(ptr)
    {
        public ObjectDestructible breakableProp;

        protected override void Awake()
        {
            base.Awake();

            m_propFrames = new List<ObjectFrame>();
            m_actionFrames = new List<ActionFrame>();
        }

        public override void OnSceneBegin()
        {
            base.OnSceneBegin();

            foreach(ActionFrame actionFrame in m_actionFrames)
            {
                actionFrame.Reset();
            }
        }

        public void SetBreakableObject(ObjectDestructible destructable)
        {
            this.breakableProp = destructable;
        }

        public void DestructionEvent()
        {
            breakableProp._isDead = false;
            breakableProp.TakeDamage(Vector3.zero, 100f, true);
            gameObject.SetActive(false);
        }
    }
}
