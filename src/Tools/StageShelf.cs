using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;

using MelonLoader;

using NEP.MonoDirector.Core;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace NEP.MonoDirector.Tools
{
    [RegisterTypeInIl2Cpp]
    public class StageShelf(IntPtr ptr) : MonoBehaviour(ptr)
    {
        private Film m_film;
        private StageShelfSocket[] m_sockets;
        private StageShelfSocket m_newStageSocket;
        private StageShelfSocket m_deleteStageSocket;
        private StageShelfSocket m_activeStageSocket;

        private Spawnable m_reelSpawnable;
        private Coroutine m_reelSpawnRoutine;

        private AudioSource m_deleteSfx;

        private void Awake()
        {
            Transform shelf = transform.Find("Canvas/Shelf");

            m_sockets = new StageShelfSocket[shelf.childCount];

            for (int i = 0; i < shelf.childCount; i++)
            {
                Transform child = shelf.GetChild(i);
                m_sockets[i] = child.GetComponent<StageShelfSocket>();
            }

            m_newStageSocket = transform.Find("Canvas/NewStage").GetComponent<StageShelfSocket>();
            m_deleteStageSocket = transform.Find("Canvas/Trash").GetComponent<StageShelfSocket>();
            m_activeStageSocket = transform.Find("Canvas/ActiveStage").GetComponent<StageShelfSocket>();
            m_deleteSfx = transform.Find("Canvas/Trash/DeleteSFX").GetComponent<AudioSource>();

            MelonCoroutines.Start(SpawnReels());
        }

        private void OnEnable()
        {
            m_film = Director.ActiveFilm;
            transform.position = Vector3.up;

            foreach (var socket in m_sockets)
            {
                socket.gameObject.SetActive(false);
                socket.OnConnected += OnReelConnected;
                socket.OnDisconnected += OnReelDisconnected;
            }

            m_newStageSocket.OnDisconnected += OnNewReel;
            m_deleteStageSocket.OnConnected += OnDeleteReel;
            m_activeStageSocket.OnConnected += OnActiveReelSet;

            UpdateSockets();
        }

        private void OnDisable()
        {
            foreach (var socket in m_sockets)
            {
                socket.OnConnected += OnReelConnected;
                socket.OnDisconnected += OnReelDisconnected;
            }

            m_newStageSocket.OnDisconnected -= OnNewReel;
            m_deleteStageSocket.OnConnected -= OnDeleteReel;
            m_activeStageSocket.OnConnected -= OnActiveReelSet;
        }

        private void OnReelConnected(StageReel reel)
        {
            if (reel == null)
            {
                return;
            }

            UpdateSockets();
        }

        private void OnReelDisconnected(StageReel reel)
        {
            if (reel == null)
            {
                return;
            }

            UpdateSockets();
        }

        private void OnNewReel(StageReel reel)
        {
            MelonCoroutines.Start(SpawnNewReel());
        }

        private void OnDeleteReel(StageReel reel)
        {
            if (m_deleteStageSocket.Reel.Stage == null)
            {
                m_deleteStageSocket.Reel.DetachFromSocket();
                m_deleteStageSocket.Reel.Despawn();
                return;
            }

            Director.ActiveFilm.RemoveStage(m_deleteStageSocket.Reel.Stage);
            Director.SetStage(Director.ActiveFilm.Stages[Director.ActiveFilm.Stages.Count - 1]);
            m_deleteStageSocket.Reel.Despawn();
            m_deleteStageSocket.Reel.SetStage(null);
            m_deleteStageSocket.Reel.AttachToSocket(m_newStageSocket);

            UpdateSockets();

            m_deleteSfx.Play();
        }

        private void OnActiveReelSet(StageReel reel)
        {
            if (reel == null)
            {
                return;
            }

            Director.SetStage(reel.Stage);
        }

        private IEnumerator SpawnNewReel()
        {
            var spawnTask = AssetSpawner.SpawnAsync(
                    m_reelSpawnable,
                    Vector3.zero,
                    Quaternion.identity,
                    new Il2CppSystem.Nullable<Vector3>(Vector3.one),
                    null,
                    false,
                    new Il2CppSystem.Nullable<int>(0)).GetAwaiter();

            while (!spawnTask.IsCompleted) yield return null;

            Poolee newReelObj = spawnTask.GetResult();
            MarrowEntity newReelEntity = newReelObj.GetComponent<MarrowEntity>();
            StageReel reel = newReelEntity.GetComponent<StageReel>();
            reel.AttachToSocket(m_newStageSocket);
            m_newStageSocket.SetReel(reel);

            yield return null;
        }

        private IEnumerator SpawnReels()
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference()
            {
                Barcode = new Barcode("NEP.MonoDirector.Spawnable.StageReel")
            };

            m_reelSpawnable = new Spawnable()
            {
                crateRef = crateRef
            };

            AssetSpawner.Register(m_reelSpawnable);

            for (int i = 0; i < m_sockets.Length; i++)
            { 
                var task = AssetSpawner.SpawnAsync(
                    m_reelSpawnable,
                    Vector3.zero,
                    Quaternion.identity,
                    new Il2CppSystem.Nullable<Vector3>(Vector3.one),
                    null,
                    false,
                    new Il2CppSystem.Nullable<int>(0)).GetAwaiter();

                while (!task.IsCompleted) yield return null;

                Poolee obj = task.GetResult();
                MarrowEntity reelEntity = obj.GetComponent<MarrowEntity>();

                reelEntity.gameObject.SetActive(false);
                m_sockets[i].SetReel(reelEntity.GetComponent<StageReel>());
            }

            for (int i = 0; i < Director.ActiveFilm.Stages.Count; i++)
            {
                m_sockets[i].Reel.SetStage(Director.ActiveFilm.Stages[i]);
                m_sockets[i].Reel.gameObject.SetActive(true);
                m_sockets[i].Reel.AttachToSocket(m_sockets[i]);
            }

            var spawnTask = AssetSpawner.SpawnAsync(
                    m_reelSpawnable,
                    Vector3.zero,
                    Quaternion.identity,
                    new Il2CppSystem.Nullable<Vector3>(Vector3.one),
                    null,
                    false,
                    new Il2CppSystem.Nullable<int>(0)).GetAwaiter();

            while (!spawnTask.IsCompleted) yield return null;

            Poolee newReelObj = spawnTask.GetResult();
            MarrowEntity newReelEntity = newReelObj.GetComponent<MarrowEntity>();
            StageReel reel = newReelEntity.GetComponent<StageReel>();
            reel.AttachToSocket(m_newStageSocket);
            m_newStageSocket.SetReel(reel);
        }

        private void UpdateSockets()
        {
            for (int i = 0; i < m_sockets.Length; i++)
            {
                m_sockets[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < m_sockets.Length && i < m_film.Stages.Count; i++)
            {
                StageShelfSocket socket = m_sockets[i];
                socket.gameObject.SetActive(true);

                if (socket.Reel)
                {
                    socket.Reel.transform.position = socket.transform.position;
                    socket.Reel.transform.rotation = socket.transform.rotation;
                }
            }

            // Set the last socket active to add new reels to the end
            m_sockets[m_film.Stages.Count].gameObject.SetActive(true);
        }
    }
}
