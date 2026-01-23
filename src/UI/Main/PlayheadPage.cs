using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class PlayheadPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Button button_Record;
        private Button button_Play;
        private Button button_Stop;

        private bool initialized = false;

        public void Initialize()
        {
            if(initialized)
            {
                return;
            }

            button_Record = transform.Find("Record").GetComponent<Button>();
            button_Play = transform.Find("Play").GetComponent<Button>();
            button_Stop = transform.Find("Stop").GetComponent<Button>();

            // button_Record.onClick.AddListener(DispatchRecord);
            // button_Play.onClick.AddListener(DispatchPlay);
            // button_Stop.onClick.AddListener(DispatchStop);
        }

        public void DispatchRecord()
        {

        }

        public void DispatchPlay()
        {

        }

        public void DispatchStop()
        {

        }
    }
}