using System;
using System.Collections;
using System.Collections.Generic;
using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class MDFooter(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Button previousPageButton;
        private Button nextPageButton;
        private Button goBackButton;
        private TextMeshProUGUI pageText;

        private ActorsPage actorsPage;

        private void Awake()
        {
            previousPageButton = transform.Find("PreviousPage").GetComponent<Button>();
            nextPageButton = transform.Find("NextPage").GetComponent<Button>();
            goBackButton = transform.Find("GoBack").GetComponent<Button>();
            pageText = transform.Find("PageText").GetComponent<TextMeshProUGUI>();
        }

        public void LinkToActorView(ActorsPage actorsPage)
        {
            this.actorsPage = actorsPage;

            previousPageButton.onClick.AddListener(new System.Action(() => OnPreviousPageClicked()));
            nextPageButton.onClick.AddListener(new System.Action(() => OnNextPageClicked()));
            goBackButton.onClick.AddListener(new System.Action(() => OnGoBackClicked()));
        }

        public void UnlinkToActorView()
        {

        }

        private void OnPreviousPageClicked()
        {
            actorsPage.PreviousPage();
            pageText.text = $"Page {actorsPage.PageIndex}/{actorsPage.PageCount}";
        }

        private void OnNextPageClicked()
        {
            actorsPage.NextPage();
            pageText.text = $"Page {actorsPage.PageIndex}/{actorsPage.PageCount}";
        }

        private void OnGoBackClicked()
        {
            actorsPage.GoBack();
        }
    }
}