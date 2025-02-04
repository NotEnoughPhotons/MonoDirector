namespace NEP.MonoDirector.Data
{
    public class KeyframeGroup<T> where T : Keyframe
    {
        public T[] Frames;
        public float Time;
        
        public virtual void SetFrames(T[] frames, float time)
        {
            Frames = new T[frames.Length];
            Time = time;

            for (int i = 0; i < Frames.Length; i++)
            {
                Frames[i] = frames[i];
                Frames[i].SetDelta(time);
            }
        }
    }
}