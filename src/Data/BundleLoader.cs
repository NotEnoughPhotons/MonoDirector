using BoneLib;
using UnityEngine;
using System.Reflection;
using MelonLoader;

namespace NEP.MonoDirector.Data
{
    public static class BundleLoader
    {
        public static GameObject FrameObject { get => m_frameObject; }
        public static GameObject PropMarkerObject { get => m_propMarkerObject; }

        public static AudioClip BeepClip { get => m_beepClip; }
        public static AudioClip PreRollClip { get => m_preRollClip; }
        public static AudioClip PostRollClip { get => m_postRollClip; }
        public static AudioClip LinkAudioClip { get => m_linkAudioClip; }

        private static AssetBundle m_uiBundle;
        private static AssetBundle m_soundBundle;

        private static GameObject m_frameObject;
        private static GameObject m_propMarkerObject;

        private static AudioClip m_beepClip;
        private static AudioClip m_preRollClip;
        private static AudioClip m_postRollClip;
        private static AudioClip m_linkAudioClip;

        public static void Initialize()
        {
            LoadUIBundle();
            m_frameObject = m_uiBundle.LoadPersistentAsset<GameObject>("Frame");
            m_propMarkerObject = m_uiBundle.LoadPersistentAsset<GameObject>("PropMarker");

            m_beepClip = m_soundBundle.LoadPersistentAsset<AudioClip>("beep");
            m_preRollClip = m_soundBundle.LoadPersistentAsset<AudioClip>("preroll");
            m_postRollClip = m_soundBundle.LoadPersistentAsset<AudioClip>("postroll");
            m_linkAudioClip = m_soundBundle.LoadPersistentAsset<AudioClip>("linkaudio");
        }

        private static void LoadUIBundle()
        {
            string resourcePath = "NEP.MonoDirector.Resources";

            if (!HelperMethods.IsAndroid())
            {
                resourcePath += ".md_ui_pc.bundle";
            }
            else
            {
                resourcePath += ".md_ui_quest.bundle";
            }

            m_uiBundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), resourcePath);
        }

        private static void LoadSoundBundle()
        {
            string resourcePath = "NEP.MonoDirector.Resources";
            resourcePath += ".md_sounds.bundle";

            m_soundBundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), resourcePath);
        }
    }
}
