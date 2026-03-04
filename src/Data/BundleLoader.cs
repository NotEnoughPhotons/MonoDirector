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

        private static AssetBundle m_bundle;

        private static GameObject m_frameObject;
        private static GameObject m_propMarkerObject;

        public static void Initialize()
        {
            LoadBundle();
            m_frameObject = m_bundle.LoadPersistentAsset<GameObject>("Frame");
            m_propMarkerObject = m_bundle.LoadPersistentAsset<GameObject>("PropMarker");
        }

        private static void LoadBundle()
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

            m_bundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), resourcePath);
        }
    }
}
