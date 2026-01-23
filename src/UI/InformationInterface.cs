using UnityEngine;
using NEP.MonoDirector.Core;

using Il2CppTMPro;
using NEP.MonoDirector.State;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class InformationInterface(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public static InformationInterface Instance { get; private set; }

        public bool ShowUI
        {
            get
            {
                return showUI;
            }
            set
            {
                showUI = value;

                ShowIcons = showUI;
                ShowTimecode = showUI;
                ShowPlaymode = showUI;
            }
        }

        public bool ShowIcons
        {
            get
            {
                return showIcons;
            }
            set
            {
                showIcons = value;
                microIconsObject?.SetActive(showIcons);
            }
        }

        public bool ShowTimecode
        {
            get
            {
                return showTimecode;
            }
            set
            {
                showTimecode = value;
                timecodeObject?.SetActive(showTimecode);
            }
        }

        public bool ShowPlaymode
        {
            get
            {
                return showPlaymode;
            }
            set
            {
                showPlaymode = value;
                playmodeObject?.SetActive(showPlaymode);
            }
        }

        private GameObject microIconsObject;
        private GameObject timecodeObject;
        private GameObject playmodeObject;
        private GameObject countdownObject;

        private GameObject micObject;
        private GameObject micOffObject;

        private TextMeshProUGUI timecodeText;
        private TextMeshProUGUI playmodeText;
        private TextMeshProUGUI countdownText;

        private Animator countdownAnimator;

        private PlayState playState;

        private bool showUI;
        private bool showIcons;
        private bool showTimecode;
        private bool showPlaymode;

        private void Awake()
        {
            Instance = this;

            microIconsObject = transform.Find("MicroIcons").gameObject;
            timecodeObject = transform.Find("Timecode").gameObject;
            playmodeObject = transform.Find("Playmode").gameObject;
            countdownObject = transform.Find("Countdown").gameObject;

            micObject = microIconsObject.transform.Find("Microphone/Mic").gameObject;
            micOffObject = microIconsObject.transform.Find("Microphone/Disabled").gameObject;

            timecodeText = timecodeObject.transform.Find("Time").GetComponent<TextMeshProUGUI>();
            playmodeText = playmodeObject.transform.Find("Mode").GetComponent<TextMeshProUGUI>();
            countdownText = countdownObject.transform.Find("Counter").GetComponent<TextMeshProUGUI>();

            countdownAnimator = countdownObject.GetComponent<Animator>();
        }

        private void Start()
        {
            Events.OnPlayStateSet += OnPlayStateSet;

            Events.OnPrePlayback += OnSceneStart;
            Events.OnPlaybackTick += OnSceneTick;
            Events.OnStopPlayback += OnSceneEnd;

            Events.OnPreRecord += OnSceneStart;
            Events.OnStartRecording += OnStartRecording;
            Events.OnRecordTick += OnSceneTick;
            Events.OnStopRecording += OnSceneEnd;

            Events.OnTimerCountdown += OnTimerCountdown;

            showIcons = false;
            showTimecode = false;
            showPlaymode = false;

            microIconsObject.SetActive(false);
            timecodeObject.SetActive(false);
            playmodeObject.SetActive(false);
            countdownObject.SetActive(false);
        }

        private void Update()
        {
            micOffObject.SetActive(!Settings.World.useMicrophone);

            Transform head = BoneLib.Player.Head;

            transform.position = Vector3.Lerp(transform.position, head.position + head.forward, 16f * Time.deltaTime);
            transform.LookAt(head);
        }

        public void OnSceneStart()
        {
            if(Director.PlayState != PlayState.Prerecording)
            {
                return;
            }

            timecodeText.text = "0s";
            countdownObject.SetActive(true);
        }

        public void OnStartRecording()
        {
            countdownObject.SetActive(false);
        }

        public void OnSceneTick()
        {
            float time = 0f;

            if(playState == PlayState.Playing)
            {
                time = Playback.Instance.PlaybackTime;
            }

            if(playState == PlayState.Recording)
            {
                time = Recorder.instance.RecordingTime;
            }

            timecodeText.text = time.ToString("0.000") + "s";
        }

        public void OnSceneEnd()
        {
            
        }

        public void OnTimerCountdown()
        {
            countdownObject.SetActive(false);
            int counter = Director.PlayState == PlayState.Prerecording ? Recorder.instance.Countdown : Playback.Instance.Countdown;
            int currentCountdown = Settings.World.delay - counter;
            countdownText.text = currentCountdown.ToString();
            countdownObject.SetActive(true);
            countdownAnimator.Play("Countdown");
        }

        private void OnPlayStateSet(PlayState playState)
        {
            this.playState = playState;
            playmodeText.text = playState.ToString();
        }
    }
}
