using System.Collections;

using NEP.MonoDirector.Actors;

using UnityEngine;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector
{
    public class Photographer
    {
        public Photographer()
        {
            Events.OnPrePhotograph += OnPrePhotograph;
            Events.OnPhotograph += OnPhotograph;
            Events.OnPostPhotograph += OnPostPhotograph;
        }

        public int Delay { get; set; }

        private Actor m_activeActor;
        private Actor m_lastActor;

        public void SetActor(MarrowAvatar avatar)
        {
            m_lastActor = m_activeActor;
            m_activeActor = new Actor(avatar);
        }

        public void OnPrePhotograph()
        {

        }

        public void OnPhotograph()
        {

        }

        public void OnPostPhotograph()
        {

        }

        public IEnumerator PhotographRoutine()
        {
            Events.OnPrePhotograph?.Invoke();

            for(int i = 0; i < Delay; i++)
            {
                Events.OnTimerCountdown?.Invoke();
                yield return new WaitForSeconds(Delay);
            }

            Events.OnPhotograph?.Invoke();

            Events.OnPostPhotograph?.Invoke();

            yield return null;
        }
    }
}
