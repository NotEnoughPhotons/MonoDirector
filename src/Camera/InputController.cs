using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class InputController : MonoBehaviour
    {
        public InputController(System.IntPtr ptr) : base(ptr) { }

        public float mouseSensitivity = 1f;
        public float mouseSmoothness = 4f;

        private Vector3 keyboardInput;
        private Vector3 mouseInput;

        private float xMouseMove = 0f;
        private float yMouseMove = 0f;

        private bool enableKeyboard = true;
        private bool enableMouse = true;

        private bool lockCursor = false;

        private void Update()
        {
            lockCursor = Input.GetMouseButton(1);
            Cursor.lockState = lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !lockCursor;
        }

        public Vector3 KeyboardMove()
        {
            if (!enableKeyboard)
            {
                return Vector3.zero;
            }

            int yNeg = Input.GetKey(KeyCode.Q) ? -1 : 0;
            int yPos = Input.GetKey(KeyCode.E) ? 1 : 0;

            if (Input.GetKey(KeyCode.A))
            {
                keyboardInput.x = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                keyboardInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                keyboardInput.x = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                keyboardInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                keyboardInput.z = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                keyboardInput.z = 0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                keyboardInput.z = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                keyboardInput.z = 0f;
            }

            keyboardInput.y = yNeg + yPos;

            return keyboardInput;
        }

        public Vector3 MouseMove()
        {
            if (!enableMouse)
            {
                return Vector3.zero;
            }

            if (xMouseMove > 0f || xMouseMove < 0f)
            {
                xMouseMove = Mathf.Lerp(xMouseMove, 0f, mouseSmoothness * Time.deltaTime);
            }

            if (yMouseMove > 0f || yMouseMove < 0f)
            {
                yMouseMove = Mathf.Lerp(yMouseMove, 0f, mouseSmoothness * Time.deltaTime);
            }

            if (!lockCursor)
            {
                return mouseInput;
            }

            float x = Input.GetAxisRaw("Mouse X");
            float y = Input.GetAxisRaw("Mouse Y");

            bool rollCam = Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1);

            if (rollCam)
            {
                mouseInput.z -= x;
            }

            xMouseMove += x * Time.deltaTime;
            yMouseMove += y * Time.deltaTime;

            mouseInput.x += xMouseMove * mouseSensitivity;
            mouseInput.y -= yMouseMove * mouseSensitivity;

            return mouseInput;
        }
    }
}
