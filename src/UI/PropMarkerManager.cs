using Il2CppSLZ.Marrow.Pool;
using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Proxy;
using NEP.MonoDirector.State;
using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class PropMarkerManager
    {
        private static GameObject container;
        private static Dictionary<Prop, GameObject> markers = new Dictionary<Prop, GameObject>();
        private static List<GameObject> loadedMarkerObjects = new List<GameObject>();
        private static List<GameObject> activeMarkers = new List<GameObject>();

        public static void Initialize()
        {
            markers = new Dictionary<Prop, GameObject>();
            loadedMarkerObjects = new List<GameObject>();
            activeMarkers = new List<GameObject>();

            container = new GameObject("[MonoDirector] - Prop Marker Container");
            container.transform.SetParent(Bootstrap.MainContainerObject.transform);

            for (int i = 0; i < 32; i++)
            {
                GameObject obj = GameObject.Instantiate(BundleLoader.PropMarkerObject);
                obj.SetActive(false);
                obj.transform.SetParent(container.transform);
                obj.transform.localPosition = Vector3.zero;
                loadedMarkerObjects.Add(obj);
            }

            Events.OnPropCreated += AddMarkerToProp;
            Events.OnPropRemoved += RemoveMarkerFromProp;

            Events.OnPlayStateSet += ShowMarkers;
        }

        public static void CleanUp()
        {
            Events.OnPropCreated -= AddMarkerToProp;
            Events.OnPropRemoved -= RemoveMarkerFromProp;

            Events.OnPlayStateSet -= ShowMarkers;

            markers.Clear();
            loadedMarkerObjects.Clear();
            activeMarkers.Clear();
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (markers.ContainsKey(prop))
            {
                return;
            }

            GameObject asset = loadedMarkerObjects.FirstOrDefault((marker) => !activeMarkers.Contains(marker));

            asset.gameObject.SetActive(true);

            asset.transform.SetParent(prop.transform);
            asset.transform.localPosition = new Vector3(0f, 0.5f, 0f);

            markers.Add(prop, asset);
            activeMarkers.Add(asset);
        }

        public static void RemoveMarkerFromProp(Prop prop)
        {
            if (!markers.ContainsKey(prop))
            {
                return;
            }

            GameObject marker = markers[prop];
            marker.gameObject.SetActive(false);
            marker.transform.parent = null;
            markers.Remove(prop);
            activeMarkers.Remove(marker.gameObject);
        }

        private static void ShowMarkers(PlayState playState)
        {
            if(playState == PlayState.Preplaying || playState == PlayState.Prerecording)
            {
                foreach (var marker in activeMarkers)
                {
                    marker.gameObject.SetActive(false);
                }
            }

            if(playState == PlayState.Stopped)
            {
                foreach (var marker in activeMarkers)
                {
                    marker.gameObject.SetActive(true);
                }
            }
        }
    }
}
