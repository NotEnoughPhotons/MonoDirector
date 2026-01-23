using BoneLib;
using UnityEngine;

using Il2CppSLZ.Marrow;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MDMenu : MonoBehaviour
    {
        public MDMenu(System.IntPtr ptr) : base(ptr) { }
        
        public static MDMenu instance { get; private set; }

        private GameObject page_Menu;
        private GameObject page_Playhead;
        private GameObject page_Actors;
        private GameObject page_Settings;
        private GameObject page_ActorPage;

        private MDFooter footer;
        private Transform contentContainer;
        private Transform pageContainer;

        private Grip grip;
        private Rigidbody rb;

        private string startingPage;
        private string currentPage;
        private string lastPage;
        
        private void Awake()
        {
            instance = this;

            rb = GetComponent<Rigidbody>();

            contentContainer = transform.Find("Content");
            pageContainer = contentContainer.GetChild(0);

            Transform footerTransform = transform.Find("Footer");
            Transform menu = pageContainer.Find("Menu");
            Transform playhead = pageContainer.Find("Playhead");
            Transform actors = pageContainer.Find("Actors");
            Transform settings = pageContainer.Find("Settings");
            Transform actorSettings = actors.Find("ActorSettings");
            Transform gripBall = transform.Find("GripBall");

            page_Menu = menu?.gameObject;
            page_Playhead = playhead?.gameObject;
            page_Actors = actors?.gameObject;
            page_Settings = settings?.gameObject;
            page_ActorPage = actorSettings?.gameObject;

            page_Menu?.AddComponent<MenuPage>().Initialize(this);
            page_Playhead?.AddComponent<PlayheadPage>().Initialize();
            page_ActorPage?.AddComponent<ActorSettingsPage>().Initialize(this);
            page_Actors?.AddComponent<ActorsPage>().Initialize(this);
            page_Settings?.AddComponent<SettingsPage>().Initialize(this);
            footer = footerTransform.gameObject.AddComponent<MDFooter>();

            startingPage = "Actors";
            footer.LinkToActorView(page_Actors.GetComponent<ActorsPage>());

            grip = gripBall.GetComponent<Grip>();

            grip.attachedHandDelegate += new System.Action<Hand>((hand) => 
            {
                rb.isKinematic = false;
            });

            grip.detachedHandDelegate += new System.Action<Hand>((hand) =>
            {
                rb.isKinematic = true;
            });
        }

        private void OnEnable()
        {
            transform.position = Player.PhysicsRig.m_chest.position + Vector3.forward;
            transform.LookAt(Player.Head);

            if(lastPage == null)
            {   
                OpenPage(startingPage);
            }
            else
            {
                OpenPage(lastPage);
            }
        }

        public void OpenPage(string page)
        {
            lastPage = currentPage;

            switch (page)
            {
                case "Menu":
                    page_Menu?.SetActive(true);
                    page_Playhead?.SetActive(false);
                    page_Actors?.SetActive(false);
                    page_ActorPage?.SetActive(false);
                    page_Settings?.SetActive(false);
                    currentPage = page;
                    break;
                case "Playhead":
                    page_Menu?.SetActive(false);
                    page_Playhead?.SetActive(true);
                    page_Actors?.SetActive(false);
                    page_ActorPage?.SetActive(false);
                    page_Settings?.SetActive(false);
                    currentPage = page;
                    break;
                case "Actors":
                    page_Menu?.SetActive(false);
                    page_Playhead?.SetActive(false);
                    page_Actors?.SetActive(true);
                    page_ActorPage?.SetActive(false);
                    page_Settings?.SetActive(false);
                    currentPage = page;
                    break;
                case "Settings":
                    page_Menu?.SetActive(false);
                    page_Playhead?.SetActive(false);
                    page_Actors?.SetActive(false);
                    page_ActorPage?.SetActive(false);
                    page_Settings?.SetActive(true);
                    currentPage = page;
                    break;
                case "ActorSettings":
                    page_Menu?.SetActive(false);
                    page_Playhead?.SetActive(false);
                    page_Actors?.SetActive(true);
                    page_ActorPage?.SetActive(true);
                    page_Settings?.SetActive(false);
                    break;
                default:
                    throw new System.Exception("Invalid page!");
            }

            print(lastPage);
        }

        public void PreviousPage()
        {
            OpenPage(lastPage);
        }

        public void Hide() => gameObject.SetActive(false);
        public void Show() => gameObject.SetActive(true);
    }
}
