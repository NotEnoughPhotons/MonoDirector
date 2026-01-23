using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class SettingsPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private MDMenu menu;

        private Button button_Audio;
        private Button button_Camera;
        private Button button_World;
        private Button button_Debug;
        private Button button_Credits;

        private GameObject page_Audio;
        private GameObject page_Camera;
        private GameObject page_Credits;

        private Transform optionsContainer;
        private Transform pageContainer;

        private bool initialized = false;

        public void Initialize(MDMenu menu)
        {
            if(initialized)
            {
                return;
            }

            optionsContainer = transform.GetChild(0);
            pageContainer = transform.GetChild(1);

            button_Audio = optionsContainer.GetChild(0).GetComponent<Button>();
            button_Camera = optionsContainer.GetChild(1).GetComponent<Button>();
            button_World = optionsContainer.GetChild(2).GetComponent<Button>();
            button_Debug = optionsContainer.GetChild(3).GetComponent<Button>();
            button_Credits = optionsContainer.GetChild(4).GetComponent<Button>();

            page_Audio = pageContainer.GetChild(0).gameObject;
            page_Camera = pageContainer.GetChild(1).gameObject;
            page_Credits = pageContainer.GetChild(2).gameObject;

            // button_Audio.onClick.AddListener(() => OpenPage("Audio"));
            // button_Camera.onClick.AddListener(() => OpenPage("Camera"));
            // button_Credits.onClick.AddListener(() => OpenPage("Credits"));

            optionsContainer.gameObject.SetActive(true);
            pageContainer.gameObject.SetActive(false);
        }

        public void OpenPage(string page)
        {
            switch (page)
            {
                case "Audio":
                    page_Audio.SetActive(true);
                    page_Camera.SetActive(false);
                    page_Credits.SetActive(false);
                    break;
                case "Camera":
                    page_Audio.SetActive(false);
                    page_Camera.SetActive(true);
                    page_Credits.SetActive(false);
                    break;
                case "Credits":
                    page_Audio.SetActive(false);
                    page_Camera.SetActive(false);
                    page_Credits.SetActive(true);
                    break;
                default:
                    throw new System.Exception("Invalid options page!");
            }

            optionsContainer.gameObject.SetActive(false);
            pageContainer.gameObject.SetActive(true);
        }
    }
}
