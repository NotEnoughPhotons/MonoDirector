using BoneLib;
using BoneLib.BoneMenu;
using Il2CppSLZ.Bonelab;
using NEP.MonoDirector.Data;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    public static class MenuBootstrap
    {
        private static PreferencesPanelView m_panelView;
        private static GameObject m_gridView;
        private static Button m_button;

        internal static void Initialize(UIRig rig)
        {
            m_panelView = rig.popUpMenu.preferencesPanelView;
            Transform pageTransform = m_panelView.pages[m_panelView.defaultPage].transform;
            m_gridView = pageTransform.Find("grid_Options").gameObject;
            InjectButton();
        }

        internal static void InjectButton()
        {
            GameObject buttonObject = GameObject.Instantiate(BundleLoader.MenuButtonObject, m_gridView.transform);
            buttonObject.SetActive(true);
            buttonObject.transform.SetSiblingIndex(6);
            m_button = buttonObject.GetComponent<Button>();

            var buttonAction = () =>
            {
                m_panelView.PAGESELECT(11);
                Menu.OpenPage(MDBoneMenu.MonoDirectorPage);
            };

            m_button.onClick.AddListener(buttonAction);
        }
    }
}
