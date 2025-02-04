using System;

namespace NEP.MonoDirector.Data
{
    public class ActionFrame
    {
        public ActionFrame(Action action, float timestamp)
        {
            this.action = action;
            this.timestamp = timestamp;
        }

        public Action action;
        public float timestamp;

        private bool runOnce = false;

        public void Reset()
        {
            runOnce = false;
        }

        public void Run()
        {
            if (runOnce == false)
            {
                action?.Invoke();
                runOnce = true;
            }
        }
    }
}
