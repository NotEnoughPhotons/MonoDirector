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

        private void Awake()
        {
            Transform shelf = transform.Find("Canvas/Shelf");

            m_sockets = new StageShelfSocket[shelf.childCount];

            for (int i = 0; i < shelf.childCount; i++)
            {
                Transform child = shelf.GetChild(i);
                m_sockets[i] = child.GetComponent<StageShelfSocket>();
            }

            MelonCoroutines.Start(SpawnReels());
        }

        private void OnEnable()
        {
            m_film = Director.ActiveFilm;
            transform.position = Vector3.up;
        }

        private IEnumerator SpawnReels()
        {
            SpawnableCrateReference crateRef = new SpawnableCrateReference()
            {
                Barcode = new Barcode("NEP.MonoDirector.Spawnable.StageReel")
            };

            Spawnable spawnable = new Spawnable()
            {
                crateRef = crateRef
            };

            AssetSpawner.Register(spawnable);

            for (int i = 0; i < m_sockets.Length; i++)
            { 
                var task = AssetSpawner.SpawnAsync(
                    spawnable,
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
        }
    }
}
