namespace NEP.MonoDirector.Proxy
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorProxy : TrackableProxy
    {
        public ActorProxy(IntPtr ptr) : base(ptr) { }
    }
}
