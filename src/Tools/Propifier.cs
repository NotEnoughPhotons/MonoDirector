using NEP.MonoDirector.Actors;
using UnityEngine;
using MelonLoader;

using Il2CppSLZ.Marrow;
using Il2CppTrees;
using Il2CppSLZ.Marrow.Interaction;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class Propifier(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public enum Mode
        {
            Prop,
            Remove
        }

        private Mode m_mode;

        private Grip m_grip;
        private Transform m_firePoint;
        private float m_range;
        private GameObject m_laser;
        private Rigidbody m_rigidbody;

        private GameObject m_propModeIcon;
        private GameObject m_removeModeIcon;

        private float m_fireForce = 5f;

        private GunSFX m_gunSFX;

        private Action<Hand> m_OnHandAttached;
        private Action<Hand> m_OnHandDetached;
        private Action<Hand> m_OnTriggerGripUpdate;

        private void Awake()
        {
            m_rigidbody = GetComponent<Rigidbody>();
            m_gunSFX = GetComponent<GunSFX>();
            m_grip = transform.Find("Grips/HandlePrimaryGrip").GetComponent<Grip>();
            m_firePoint = transform.Find("FirePoint");
            m_laser = transform.Find("Laser Pointer").gameObject;
            m_range = 30;

            m_propModeIcon = transform.Find("Art/ScreenMode_Prop").gameObject;
            m_removeModeIcon = transform.Find("Art/ScreenMode_Remove").gameObject;

            m_OnHandAttached = OnHandAttached;
            m_OnHandDetached = OnHandDetached;
            m_OnTriggerGripUpdate = OnTriggerGripUpdate;
        }

        private void OnEnable()
        {
            m_grip.attachedHandDelegate += m_OnHandAttached;
            m_grip.detachedHandDelegate += m_OnHandDetached;
            m_grip.attachedUpdateDelegate += m_OnTriggerGripUpdate;
        }

        private void OnDisable()
        {
            m_grip.attachedHandDelegate -= m_OnHandAttached;
            m_grip.detachedHandDelegate -= m_OnHandDetached;
            m_grip.attachedUpdateDelegate -= m_OnTriggerGripUpdate;
        }

        private void PrimaryButtonDown()
        {
            m_gunSFX.GunShot();
            m_rigidbody.AddForce(m_rigidbody.transform.up - m_firePoint.forward * m_fireForce, ForceMode.Impulse);

            if(Physics.Raycast(m_firePoint.position, m_firePoint.forward * m_range, out RaycastHit hit))
            {
                if (hit.rigidbody == null)
                    return;

                if (!MarrowBody.Cache.TryGet(hit.rigidbody.gameObject, out MarrowBody body))
                    return;

                MarrowEntity entity = body.Entity;

                if (entity == null)
                    return;

                if(m_mode == Mode.Prop)
                    PropBuilder.BuildProp(entity);
                else
                    PropBuilder.RemoveProp(entity);
            }
        }

        private void OnHandAttached(Hand hand)
        {
            m_laser.SetActive(true);
        }

        private void OnHandDetached(Hand hand)
        {
            m_laser.SetActive(false);
        }

        private void OnTriggerGripUpdate(Hand hand)
        {
            bool bTapped = hand.Controller.GetMenuTap();

            if (bTapped)
            {
                if(m_mode == Mode.Prop)
                {
                    SetMode(Mode.Remove);
                }
                else
                {
                    SetMode(Mode.Prop);
                }
            }

            if (hand._indexButtonDown)
            {
                PrimaryButtonDown();
            }
        }

        private void SetMode(Mode mode)
        {
            this.m_mode = mode;
            m_gunSFX.DryFire();

            if (mode == Mode.Prop)
            {
                m_propModeIcon.SetActive(true);
                m_removeModeIcon.SetActive(false);
            }
            else if (mode == Mode.Remove)
            {
                m_propModeIcon.SetActive(false);
                m_removeModeIcon.SetActive(true);
            }
        }
    }
}