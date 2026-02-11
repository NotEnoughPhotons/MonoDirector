using UnityEngine;
using MelonLoader;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Actors;
using Il2CppTMPro;
using UnityEngine.UI;

namespace NEP.MonoDirector.UI
{
    [RegisterTypeInIl2Cpp]
    public class ActorPanel(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Transform m_root;

        private TextMeshProUGUI m_actorNameText;
        private Button m_recastButton;
        private Button m_deleteButton;

        private Action m_onRecastClicked;
        private Action m_onDeleteClicked;

        private Vector3 m_targetPosition;

        private void Awake()
        {
            m_root = transform.GetChild(0);

            m_actorNameText = m_root.Find("ActorName").GetComponent<TextMeshProUGUI>();
            m_recastButton = m_root.Find("Management/Recast").GetComponent<Button>();
            m_deleteButton = m_root.Find("Management/Delete").GetComponent<Button>();

            m_onRecastClicked = OnRecastClicked;
            m_onDeleteClicked = OnDeleteClicked;

            m_root.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            Director.OnActorSelected += OnActorSelected;
            Director.OnActorDeselected += OnActorDeselected;

            m_recastButton.onClick.AddListener(m_onRecastClicked);
            m_deleteButton.onClick.AddListener(m_onDeleteClicked);
        }

        private void OnDisable()
        {
            Director.OnActorSelected -= OnActorSelected;
            Director.OnActorDeselected -= OnActorDeselected;

            m_recastButton.onClick.RemoveListener(m_onRecastClicked);
            m_deleteButton.onClick.RemoveListener(m_onDeleteClicked);
        }

        private void Update()
        {
            transform.position = Vector3.Lerp(transform.position, m_targetPosition, 8f * Time.deltaTime);
        }

        private void OnActorSelected(Actor actor)
        {
            m_root.gameObject.SetActive(true);

            m_targetPosition = actor.ClonedAvatar.transform.position + (Vector3.right + Vector3.up);

            m_actorNameText.text = actor.ActorName;
        }

        private void OnActorDeselected(Actor actor)
        {
            m_root.gameObject.SetActive(false);
        }

        private void OnRecastClicked()
        {
            Director.Recast(Director.SelectedActor);
            m_root.gameObject.SetActive(false);
        }

        private void OnDeleteClicked()
        {
            Director.RemoveActor(Director.SelectedActor);
            m_root.gameObject.SetActive(false);
        }
    }
}
