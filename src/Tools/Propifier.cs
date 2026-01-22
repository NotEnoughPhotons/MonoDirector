using NEP.MonoDirector.Actors;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.Tools
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class Propifier : MonoBehaviour
    {
        public Propifier(System.IntPtr ptr) : base(ptr) { }

        public enum Mode
        {
            Prop,
            Remove
        }

        public Mode mode;

        public TargetGrip triggerGrip;
        public Transform firePoint;
        public float maxRange;
        public GameObject laserPointer;
        public Rigidbody rigidbody;

        public GameObject propModeIcon;
        public GameObject removeModeIcon;

        public float fireForce = 5f;

        private GunSFX gunSFX;

        private void Awake()
        {
            gunSFX = GetComponent<GunSFX>();
            
            triggerGrip = transform.Find("Grips/HandlePrimaryGrip").GetComponent<TargetGrip>();
            firePoint = transform.Find("PointOfInterest/FirePoint");
            laserPointer = transform.Find("Propifier Art/Laser").gameObject;
            maxRange = 30;

            propModeIcon = transform.Find("Propifier Art/ScreenMode_Prop").gameObject;
            removeModeIcon = transform.Find("Propifier Art/ScreenMode_Remove").gameObject;
        }

        private void Start()
        {
            triggerGrip.attachedHandDelegate += new System.Action<Hand>((hand) => OnAttachHand());
            triggerGrip.detachedHandDelegate += new System.Action<Hand>((hand) => OnDetachHand());
            triggerGrip.attachedUpdateDelegate += new System.Action<Hand>((hand) => OnTriggerGripUpdate());
        }

        private void PrimaryButtonDown()
        {
            gunSFX.GunShot();
            rigidbody.AddForce(rigidbody.transform.up - firePoint.forward * fireForce, ForceMode.Impulse);

            if(Physics.Raycast(firePoint.position, firePoint.forward * maxRange, out RaycastHit hit))
            {
                if(hit.rigidbody == null)
                {
                    return;
                }

                InteractableHost entity = hit.rigidbody.GetComponent<InteractableHost>();

                if(entity == null)
                {
                    return;
                }

                if(mode == Mode.Prop)
                {
                    PropBuilder.BuildProp(entity);
                }
                else
                {
                    PropBuilder.RemoveProp(entity);
                }
            }
        }

        public void OnAttachHand()
        {
            rigidbody = GetComponent<Rigidbody>();
            laserPointer.SetActive(true);
        }

        public void OnDetachHand()
        {
            rigidbody = GetComponent<Rigidbody>();
            laserPointer.SetActive(false);
        }

        public void OnTriggerGripUpdate()
        {
            Hand hand = triggerGrip.GetHand();
            bool bTapped = hand.Controller.GetMenuTap();

            if (bTapped)
            {
                if(mode == Mode.Prop)
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

        public void SetMode(Mode mode)
        {
            this.mode = mode;
            gunSFX.DryFire();

            if (mode == Mode.Prop)
            {
                propModeIcon.SetActive(true);
                removeModeIcon.SetActive(false);
            }
            else if (mode == Mode.Remove)
            {
                propModeIcon.SetActive(false);
                removeModeIcon.SetActive(true);
            }
        }
    }
}