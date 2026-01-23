using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MenuPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private MDMenu menu;

        private Button button_Playhead;
        private Button button_Actors;
        private Button button_Settings;
        private Button button_Exit;

        private bool initialized = false;

        public void Initialize(MDMenu menu)
        {
            if (initialized)
            {
                return;
            }
            
            button_Playhead = transform.GetChild(0).GetComponent<Button>();
            button_Actors = transform.GetChild(1).GetComponent<Button>();
            button_Settings = transform.GetChild(2).GetComponent<Button>();
            button_Exit = transform.GetChild(3).GetComponent<Button>();

            //button_Playhead.onClick.AddListener(() => menu.OpenPage("Playhead"));
            //button_Actors.onClick.AddListener(() => menu.OpenPage("Actors"));
            //button_Settings.onClick.AddListener(() => menu.OpenPage("Settings"));
            //button_Exit.onClick.AddListener(() => menu.Hide());
        }

        public void SetRoot(MDMenu menu)
        {
            this.menu = menu;
        }
    }
}
