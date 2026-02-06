using Il2CppTMPro;
using UnityEngine;
using UnityEngine.UI;

using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.UI
{
    [MelonLoader.RegisterTypeInIl2Cpp]
    public class ActorsPage(IntPtr ptr) : MonoBehaviour(ptr)
    {
        public int PageIndex => pageIndex + 1;
        public int PageCount => displayPages.Count;

        private List<ActorDisplayPage> displayPages;
        private ActorDisplayPage currentDisplayPage;

        private MDMenu menu;

        private ActorSettingsPage settingsPage;

        private ActorEntry[] actorEntries;

        private Transform castListContainer;

        private bool initialized = false;

        private int pageIndex = 0;

        private void Start()
        {
            Events.OnActorCasted += OnActorCreated;
            Events.OnActorUncasted += OnActorRemoved;
        }

        public void Initialize(MDMenu menu)
        {
            if (initialized)
            {
                return;
            }

            this.menu = menu;
            castListContainer = transform.GetChild(0);
            settingsPage = transform.GetChild(1).GetComponent<ActorSettingsPage>();
            actorEntries = new ActorEntry[castListContainer.childCount];

            for (int i = 0; i < actorEntries.Length; i++)
            {
                GameObject current = castListContainer.GetChild(i).gameObject;
                ActorEntry entry = current.AddComponent<ActorEntry>();

                entry.avatarImage = current.transform.GetChild(0).GetComponent<RawImage>();
                entry.avatarNameText = current.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
                entry.avatarButton = current.GetComponent<Button>();

                entry.avatarButton.onClick.AddListener(new System.Action(() => OnActorSelected(entry)));

                actorEntries[i] = entry;

                current.SetActive(false);
            }

            castListContainer.gameObject.SetActive(true);
            settingsPage.gameObject.SetActive(false);

            displayPages = new List<ActorDisplayPage>();

            currentDisplayPage = new ActorDisplayPage();
            currentDisplayPage.LinkToView(this);

            displayPages.Add(currentDisplayPage);
        }

        public void OnActorCreated(Actor actor)
        {
            ActorEntry first = GetFirst();
            UpdateEntry(actor, first);

            displayPages?.Last().AddActor(actor);
        }

        public void OnActorRemoved(Actor actor)
        {
            ActorEntry target = null;

            for(int i = 0; i < actorEntries.Length; i++)
            {
                if (actorEntries[i].gameObject.activeSelf && actorEntries[i].GetActor() == actor)
                {
                    target = actorEntries[i];
                    break;
                }
            }

            if (target == null)
            {
                Logging.Warn("Target actor entry not found! There might be a problem with the UI.");
                return;
            }

            target.Hide();
        }

        public void OnPageFilled(Actor actor)
        {
            ActorDisplayPage emptyPage = new ActorDisplayPage();
            emptyPage.LinkToView(this);
            emptyPage.AddActor(actor);
            displayPages.Add(emptyPage);
        }

        public void PreviousPage()
        {
            if(pageIndex == 0)
            {
                pageIndex = 0;
                return;
            }

            ClearEntries();
            UpdatePage(displayPages[--pageIndex]);
        }

        public void NextPage()
        {
            if (pageIndex == displayPages.Count - 1)
            {
                pageIndex = displayPages.Count - 1;
                return;
            }

            ClearEntries();
            UpdatePage(displayPages[++pageIndex]);
        }

        public void GoBack()
        {
            settingsPage.gameObject.SetActive(false);
            castListContainer.gameObject.SetActive(true);
        }

        private void UpdatePage(ActorDisplayPage page)
        {
            currentDisplayPage = page;
            int numEntries = currentDisplayPage.actorEntries.Count;

            for(int i = 0; i < numEntries; i++)
            {
                UpdateEntry(currentDisplayPage.actorEntries[i], actorEntries[i]);
            }
        }

        private void OnActorSelected(ActorEntry entry)
        {
            castListContainer.gameObject.SetActive(false);
            settingsPage.UpdateInformation(entry.GetActor());
            menu.OpenPage("ActorSettings");
        }

        private void UpdateEntry(Actor actor, ActorEntry entry)
        {
            if(entry == null)
            {
                return;
            }
            
            // entry.avatarImage.texture = actor.actorPortrait;
            entry.avatarNameText.text = actor.ActorName;
            entry.SetActor(actor);
            entry.gameObject.SetActive(true);
        }

        private void ClearEntries()
        {
            for(int i = 0; i < actorEntries.Length; i++)
            {
                actorEntries[i].gameObject.SetActive(false);
            }
        }

        private ActorEntry GetFirst()
        {
            for (int i = 0; i < actorEntries.Length; i++)
            {
                if (!actorEntries[i].gameObject.activeInHierarchy)
                {
                    return actorEntries[i];
                }
            }

            return null;
        }
    }
}