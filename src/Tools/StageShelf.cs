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

        private Spawnable m_reelSpawnable;
        private Coroutine m_reelSpawnRoutine;

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

            MelonCoroutines.Start(SpawnReels());
        }

        private void OnEnable()
        {
            m_film = Director.ActiveFilm;
            transform.position = Vector3.up;

            m_newStageSocket.OnDisconnected += OnNewStage;
            m_deleteStageSocket.OnConnected += OnDeleteStage;
        }

        private void OnDisable()
        {
            m_newStageSocket.OnDisconnected -= OnNewStage;
            m_deleteStageSocket.OnConnected -= OnDeleteStage;
        }

        private void OnNewStage()
        {
            MelonCoroutines.Start(SpawnNewReel());
        }

        private void OnDeleteStage()
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
    }
}
