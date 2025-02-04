namespace NEP.MonoDirector.Data
{
    public class Keyframe
    {
        public float frameTime;
        public bool activeThisFrame;

        public void SetDelta(float time)
        {
            frameTime = time;
        }
    }
}