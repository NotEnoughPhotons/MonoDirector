using BoneLib;
using UnityEngine;
using System.Reflection;

namespace NEP.MonoDirector.Data
{
    public static class BundleLoader
    {
        public static GameObject FrameObject { get => m_frameObject; }

        private static AssetBundle m_bundle;

        private static GameObject m_frameObject;

        public static void Initialize()
        {
            m_bundle = HelperMethods.LoadEmbeddedAssetBundle(Assembly.GetExecutingAssembly(), "NEP.MonoDirector.Resources.md_ui_pc.bundle");

            m_frameObject = m_bundle.LoadPersistentAsset<GameObject>("Frame");
        }
    }
}
