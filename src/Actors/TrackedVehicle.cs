using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class TrackedVehicle(IntPtr ptr) : Prop(ptr)
    {
        public Atv Vehicle { get => m_vehicle; }

        protected Atv m_vehicle;

        public void SetVehicle(Atv vehicle)
        {
            this.m_vehicle = vehicle;
        }

        public void RemoveVehicle()
        {
            m_vehicle = null;

            SetPhysicsActive(true);
        }

        public override void OnSceneBegin()
        {
            if (PropFrames == null)
                return;

            if (PropFrames.Count == 0)
                return;

            SetPhysicsActive(false);
        }

        public override void Act()
        {
            gameObject.SetActive(true);
        }

        public override void Record(int frame)
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = transform,
                position = transform.position,
                rotation = transform.rotation,
                frameTime = Recorder.Instance.RecordingTime
            };

            m_bodyFrames.Add(objectFrame);
        }
    }
}
