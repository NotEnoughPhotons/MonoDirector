using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorSettingsPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private MDMenu menu;

        private RawImage actorPortrait;
        private TextMeshProUGUI actorNameText;
        private TextMeshProUGUI visiblityButtonText;

        private Button visibilityButton;
        private Button recastButton;
        private Button deleteButton;
        private Button deletePropsButton;

        private Actor actor;

        private bool initialized = false;

        public void Initialize(MDMenu menu)
        {
            if (initialized)
            {
                return;
            }

            this.menu = menu;

            actorPortrait = transform.GetChild(0).GetComponent<RawImage>();
            actorNameText = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
            Transform optionsGroup = transform.Find("OptionsGroup");
            visibilityButton = optionsGroup.GetChild(0).GetComponent<Button>();
            visiblityButtonText = visibilityButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            recastButton = optionsGroup.GetChild(1).GetComponent<Button>();
            deleteButton = optionsGroup.GetChild(2).GetComponent<Button>();
            deletePropsButton = optionsGroup.GetChild(3).GetComponent<Button>();

            visibilityButton.onClick.AddListener(new System.Action(() => OnShowButtonClicked()));
            recastButton.onClick.AddListener(new System.Action(() => OnRecastButtonClicked()));
            deleteButton.onClick.AddListener(new System.Action(() => OnDeleteButtonClicked()));
            deletePropsButton.onClick.AddListener(new System.Action(() => OnDeletePropsButtonClicked()));
        }

        public void UpdateInformation(Actor actor)
        {
            this.actor = actor;
            // actorPortrait.texture = this.actor.actorPortrait;
            actorNameText.text = this.actor.ActorName;
        }

        public void OnDeleteButtonClicked()
        {
            actor.Delete();
            MDMenu.Instance.PreviousPage();
        }

        public void OnDeletePropsButtonClicked()
        {
            Director.ClearLastProps();
        }

        public void OnRecastButtonClicked()
        {
            Director.Recast(actor);
        }

        public void OnShowButtonClicked()
        {
            bool lastShow = actor.ClonedAvatar.gameObject.activeInHierarchy;
            visiblityButtonText.text = lastShow ? "Show" : "Hide";
            actor.ClonedAvatar.gameObject.SetActive(!lastShow);
        }
    }
}