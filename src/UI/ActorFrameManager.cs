using BoneLib;
using Il2CppSLZ.Marrow.Data;
using Il2CppSLZ.Marrow.Pool;
using Il2CppSLZ.Marrow.Warehouse;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Proxy;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class ActorFrameManager
    {
        private static GameObject container;
        private static Dictionary<ActorProxy, GameObject> frames;
        private static List<GameObject> loadedFrameObjects;
        private static List<GameObject> activeFrames;

        public static void Initialize()
        {
            frames = new Dictionary<ActorProxy, GameObject>();
            loadedFrameObjects = new List<GameObject>();
            activeFrames = new List<GameObject>();

            container = new GameObject("[MonoDirector] - Actor Frame Container");
            container.transform.SetParent(Bootstrap.MainContainerObject.transform);

            for (int i = 0; i < 32; i++)
            {
                GameObject obj = GameObject.Instantiate(BundleLoader.FrameObject);
                obj.SetActive(false);
                obj.transform.SetParent(container.transform);
                obj.transform.localPosition = Vector3.zero;
                loadedFrameObjects.Add(obj);
            }
        }

        public static void CleanUp()
        {
            frames.Clear();
            loadedFrameObjects.Clear();
            activeFrames.Clear();
        }

        public static GameObject AddFrameToActor(ActorProxy proxy)
        {
            if (frames.ContainsKey(proxy))
            {
                return null;
            }

            GameObject asset = loadedFrameObjects.FirstOrDefault((frame) => !activeFrames.Contains(frame));

            asset.gameObject.SetActive(true);

            asset.transform.SetParent(proxy.Collider.transform);
            asset.transform.localPosition = Vector3.zero;
            asset.transform.localScale = proxy.Collider.size;

            frames.Add(proxy, asset);
            activeFrames.Add(asset);

            return asset;
        }

        public static void RemoveFrameFromActor(ActorProxy proxy)
        {
            if (!frames.ContainsKey(proxy))
            {
                return;
            }

            Poolee frame = frames[proxy].GetComponent<Poolee>();
            frame.Despawn();
            frame.gameObject.SetActive(false);
            frame.transform.parent = container.transform;
            frame.transform.localScale = Vector3.one;
            frames.Remove(proxy);
            activeFrames.Remove(frame.gameObject);
        }

        internal static void OnFrameSpawned(GameObject frameObject)
        {
            frameObject.SetActive(false);
            frameObject.transform.SetParent(container.transform);
            loadedFrameObjects.Add(frameObject);
        }
    }
}
