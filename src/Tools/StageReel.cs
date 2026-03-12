using UnityEngine;
using MelonLoader;
using NEP.MonoDirector.Core;
using Il2CppTMPro;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Pool;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageReel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Stage m_stage;
        private Rigidbody m_rigidbody;
        private TextMeshPro m_title;
        private StageShelfSocket m_attachedSocket;
        private StageShelfSocket m_lastConnectedSocket;
        private StageShelfSocket m_hoveredSocket;
        private Grip m_grip;
        private Poolee m_poolee;

        private Action<Hand> m_onHandAttached;
        private Action<Hand> m_onHandReleased;

        private bool m_hackDespawnFlag;

        private void Awake()
        {
            m_title = transform.Find("Text").GetComponent<TextMeshPro>();
            m_grip = transform.Find("Grip").GetComponent<Grip>();
            m_poolee = GetComponent<Poolee>();
            m_rigidbody = GetComponent<Rigidbody>();

            m_onHandAttached = OnHandAttached;
            m_onHandReleased = OnHandReleased;

            m_rigidbody.isKinematic = true;
        }

        protected void OnEnable()
        {
            m_grip.attachedHandDelegate += m_onHandAttached;
            m_grip.detachedHandDelegate += m_onHandReleased;
        }

        protected void OnDisable()
        {
            m_grip.attachedHandDelegate -= m_onHandAttached;
            m_grip.detachedHandDelegate -= m_onHandReleased;
        }

        private void Update()
        {
            if (m_hackDespawnFlag)
            {
                m_poolee.Despawn();
                m_hackDespawnFlag = false;
            }
        }

        private void OnTriggerEnter(Collider collider)
        {
            StageShelfSocket socket = collider.GetComponent<StageShelfSocket>();

            if (socket == null)
            {
                return;
            }

            m_hoveredSocket = socket;

            if (m_hoveredSocket.Empty)
            {
                m_hoveredSocket.HoverOver();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            StageShelfSocket socket = collider.GetComponent<StageShelfSocket>();

            if (socket == null)
            {
                return;
            }

            if (m_hoveredSocket)
            {
                m_hoveredSocket.HoverAway();
                m_hoveredSocket = null;
            }
        }

        private void Despawn()
        {
            // Directly despawning a poolee while in your hands triggers a stack overflow.
            // This is because Poolee.Despawn invokes Grip.detachedHandDelegate.
            // Anyone who calls Poolee.Despawn inside of code that Grip.detachedHandDelegate is subscribed to, will trigger a stack overflow.
            // To get around this, we'll just set a flag to true and check it in the Update loop for despawning.
            // I really wish I didn't have to do this.
            m_hackDespawnFlag = true;
        }

        public void SetStage(Stage stage)
        {
            m_stage = stage;
            m_title.text = m_stage.Name;
        }

        public void AttachToSocket(StageShelfSocket socket)
        {
            m_attachedSocket = socket;
            m_lastConnectedSocket = m_attachedSocket;
            m_attachedSocket.Connect();
            m_rigidbody.isKinematic = true;
            transform.position = m_attachedSocket.transform.position;
            transform.rotation = m_attachedSocket.transform.rotation;
        }

        public void DetachFromSocket()
        {
            if (!m_attachedSocket)
            {
                return;
            }

            if (m_attachedSocket.IsDisconnected)
            {
                return;
            }

            m_attachedSocket.Disconnect();
            m_attachedSocket = null;
            m_rigidbody.isKinematic = false;
        }

        private void OnHandAttached(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
            {
                return;
            }

            DetachFromSocket();
        }

        private void OnHandReleased(Hand hand)
        {
            if (m_grip.attachedHands.Count > 1)
            {
                return;
            }

            // Already hovering over a socket?
            if (m_hoveredSocket)
            {
                // If there's no stage already, make one.
                if (m_stage == null)
                {
                    m_stage = new Stage("Stage");
                    SetStage(m_stage);
                    Director.SetStage(m_stage);
                    Director.ActiveFilm.AddStage(m_stage);
                }

                AttachToSocket(m_hoveredSocket);
            }
            // If we let go of the reel, it should go back to the previously connected socket.
            else if (m_lastConnectedSocket)
            {
                if (m_stage == null)
                {
                    Despawn();
                }

                AttachToSocket(m_lastConnectedSocket);
            }
        }
    }
}
