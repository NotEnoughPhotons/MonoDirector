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

        private Prop m_steeringWheelProp;
        private Prop m_frontAxleProp;
        private Prop m_backLeftWheelProp;
        private Prop m_backRightWheelProp;
        private Prop m_frontLeftWheelProp;
        private Prop m_frontRightWheelProp;

        public void SetVehicle(Atv vehicle)
        {
            this.vehicle = vehicle;

            m_steeringWheelProp = vehicle.steeringWheel.gameObject.AddComponent<Prop>();
            m_frontAxleProp = vehicle.frontAxle.gameObject.AddComponent<Prop>();
            m_backLeftWheelProp = vehicle._backLfRb.gameObject.AddComponent<Prop>();
            m_backRightWheelProp = vehicle._backRtRb.gameObject.AddComponent<Prop>();
            m_frontLeftWheelProp = vehicle._frontLfRb.gameObject.AddComponent<Prop>();
            m_frontRightWheelProp = vehicle._frontRtRb.gameObject.AddComponent<Prop>();
        }

        public void RemoveVehicle()
        {
            vehicle = null;

            InteractableRigidbody.isKinematic = false;
            
            m_steeringWheelProp.InteractableRigidbody.isKinematic = false;
            m_frontAxleProp.InteractableRigidbody.isKinematic = false;
            m_backLeftWheelProp.InteractableRigidbody.isKinematic = false;
            m_backRightWheelProp.InteractableRigidbody.isKinematic = false;
            m_frontLeftWheelProp.InteractableRigidbody.isKinematic = false;
            m_frontRightWheelProp.InteractableRigidbody.isKinematic = false;
            
            Destroy(m_steeringWheelProp);
            Destroy(m_frontAxleProp);
            Destroy(m_backLeftWheelProp);
            Destroy(m_backRightWheelProp);
            Destroy(m_frontLeftWheelProp);
            Destroy(m_frontRightWheelProp);
            
            
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

            InteractableRigidbody.isKinematic = true;
            
            m_steeringWheelProp.OnSceneBegin();
            m_frontAxleProp.OnSceneBegin();
            m_backLeftWheelProp.OnSceneBegin();
            m_backRightWheelProp.OnSceneBegin();
            m_frontLeftWheelProp.OnSceneBegin();
            m_frontRightWheelProp.OnSceneBegin();

            InteractableRigidbody.position = PropFrames[0].position;
            InteractableRigidbody.rotation = PropFrames[0].rotation;
        }

        public override void Act()
        {
            gameObject.SetActive(true);

            InteractableRigidbody.isKinematic = true;

            vehicle.mainBody.position = Interpolator.InterpolatePosition(PropFrames);
            vehicle.mainBody.rotation = Interpolator.InterpolateRotation(PropFrames);
            
            m_steeringWheelProp.Act();
            m_frontAxleProp.Act();
            m_backLeftWheelProp.Act();
            m_backRightWheelProp.Act();
            m_frontLeftWheelProp.Act();
            m_frontRightWheelProp.Act();
        }

        public override void Record(int frame)
        {
            ObjectFrame objectFrame = new ObjectFrame()
            {
                transform = InteractableRigidbody.transform,
                position = InteractableRigidbody.position,
                rotation = InteractableRigidbody.rotation,
                rigidbodyVelocity = InteractableRigidbody.velocity,
                frameTime = Recorder.Instance.RecordingTime
            };

            m_propFrames.Add(objectFrame);
            
            m_steeringWheelProp.Record(frame);
            m_frontAxleProp.Record(frame);
            m_backLeftWheelProp.Record(frame);
            m_backRightWheelProp.Record(frame);
            m_frontLeftWheelProp.Record(frame);
            m_frontRightWheelProp.Record(frame);
        }
    }
}
