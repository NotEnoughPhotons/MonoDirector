using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Actors
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class TrackedVehicle(IntPtr ptr) : Prop(ptr)
    {
        public Atv Vehicle { get => vehicle; }

        protected Atv vehicle;

        public void SetVehicle(Atv vehicle)
        {
            this.vehicle = vehicle;
        }

        public override void OnSceneBegin()
        {
            if (PropFrames == null)
            {
                return;
            }

            if (PropFrames.Count == 0)
            {
                return;
            }

            vehicle.mainBody.isKinematic = true;
            vehicle.steeringWheel.isKinematic = true;
            vehicle.frontAxle.isKinematic = true;
            vehicle._backLfRb.isKinematic = true;
            vehicle._backRtRb.isKinematic = true;
            vehicle._frontLfRb.isKinematic = true;
            vehicle._frontRtRb.isKinematic = true;

            InteractableRigidbody.position = PropFrames[0].position;
            InteractableRigidbody.rotation = PropFrames[0].rotation;
        }

        public override void Act()
        {
            gameObject.SetActive(true);

            vehicle.mainBody.isKinematic = false;
            vehicle.steeringWheel.isKinematic = false;
            vehicle.frontAxle.isKinematic = false;
            vehicle._backLfRb.isKinematic = false;
            vehicle._backRtRb.isKinematic = false;
            vehicle._frontLfRb.isKinematic = false;
            vehicle._frontRtRb.isKinematic = false;

            vehicle.mainBody.velocity = Interpolator.InterpolateVelocity(PropFrames);
            vehicle.mainBody.rotation = Interpolator.InterpolateRotation(PropFrames);
        }

        public override void Record(int frame)
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = InteractableRigidbody.transform,
                position = InteractableRigidbody.position,
                rotation = InteractableRigidbody.rotation,
                rigidbodyVelocity = InteractableRigidbody.velocity,
                frameTime = Recorder.instance.RecordingTime
            };

            propFrames.Add(objectFrame);
        }
    }
}
