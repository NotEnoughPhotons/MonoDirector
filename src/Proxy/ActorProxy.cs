using NEP.MonoDirector.Actors;

namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorProxy(IntPtr ptr) : TrackableProxy(ptr)
    {
        public Actor Actor { get => m_actor; }
        private Actor m_actor;

        public void SetActor(Actor actor)
        {
            m_actor = actor;
        }
    }
}
