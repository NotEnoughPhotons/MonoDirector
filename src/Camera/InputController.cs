using UnityEngine;

namespace NEP.MonoDirector.Cameras
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class InputController(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public float mouseSensitivity = 1f;
        public float mouseSmoothness = 4f;

        private Vector3 m_keyInput;
        private Vector3 m_mouseInput;

        private float m_xMouseMove = 0f;
        private float m_yMouseMove = 0f;

        private bool m_enableKeyboard = true;
        private bool m_enableMouse = true;

        private bool m_lockCursor = false;

        private void Update()
        {
            m_lockCursor = Input.GetMouseButton(1);
            Cursor.lockState = m_lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !m_lockCursor;
        }

        public Vector3 KeyboardMove()
        {
            if (!m_enableKeyboard)
            {
                return Vector3.zero;
            }

            int yNeg = Input.GetKey(KeyCode.Q) ? -1 : 0;
            int yPos = Input.GetKey(KeyCode.E) ? 1 : 0;

            if (Input.GetKey(KeyCode.A))
            {
                m_keyInput.x = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.A))
            {
                m_keyInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.D))
            {
                m_keyInput.x = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.D))
            {
                m_keyInput.x = 0f;
            }

            if (Input.GetKey(KeyCode.W))
            {
                m_keyInput.z = 1f;
            }
            else if (Input.GetKeyUp(KeyCode.W))
            {
                m_keyInput.z = 0f;
            }

            if (Input.GetKey(KeyCode.S))
            {
                m_keyInput.z = -1f;
            }
            else if (Input.GetKeyUp(KeyCode.S))
            {
                m_keyInput.z = 0f;
            }

            m_keyInput.y = yNeg + yPos;

            return m_keyInput;
        }

        public Vector3 MouseMove()
        {
            if (!m_enableMouse)
            {
                return Vector3.zero;
            }

            if (m_xMouseMove > 0f || m_xMouseMove < 0f)
            {
                m_xMouseMove = Mathf.Lerp(m_xMouseMove, 0f, mouseSmoothness * Time.deltaTime);
            }

            if (m_yMouseMove > 0f || m_yMouseMove < 0f)
            {
                m_yMouseMove = Mathf.Lerp(m_yMouseMove, 0f, mouseSmoothness * Time.deltaTime);
            }

            if (!m_lockCursor)
            {
                return m_mouseInput;
            }

            float x = Input.GetAxisRaw("Mouse X");
            float y = Input.GetAxisRaw("Mouse Y");

            bool rollCam = Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButton(1);

            if (rollCam)
            {
                m_mouseInput.z -= x;
            }

            m_xMouseMove += x * Time.deltaTime;
            m_yMouseMove += y * Time.deltaTime;

            m_mouseInput.x += m_xMouseMove * mouseSensitivity;
            m_mouseInput.y -= m_yMouseMove * mouseSensitivity;

            return m_mouseInput;
        }
    }
}
