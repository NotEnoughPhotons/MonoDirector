using Il2CppSLZ.Marrow;
using UnityEngine;

namespace NEP.MonoDirector.Data
{
    public class ControllerKeyframe : Keyframe
    {
        public ControllerKeyframe(BaseController controller, float delta = 0.0f)
        {
            position = controller.transform.position;
            rotation = controller.transform.rotation;
            thumb = controller.GetThumbCurlAxis();
            index = controller.GetIndexCurlAxis();
            middle = controller.GetMiddleCurlAxis();
            ring = controller.GetRingCurlAxis();
            pinky = controller.GetPinkyCurlAxis();
            aPressed = controller._aButton;
            bPressed = controller._bButton;
            trackpadAxis = controller._touchPadAxis;
            joystickAxis = controller._thumbstickAxis;

            SetDelta(delta);
        }
        
        public Vector3 position;
        public Quaternion rotation;

        public float thumb;
        public float index;
        public float middle;
        public float ring;
        public float pinky;

        public bool aPressed;
        public bool bPressed;

        public Vector2 trackpadAxis;
        public Vector2 joystickAxis;
    }
}