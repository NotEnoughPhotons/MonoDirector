using NEP.MonoDirector.Actors;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.State;

using UnityEngine;

namespace NEP.MonoDirector.UI
{
    public static class PropMarkerManager
    {
        private static GameObject container;
        private static Dictionary<Prop, PropMarker> markers = new Dictionary<Prop, PropMarker>();
        private static List<PropMarker> loadedMarkerObjects = new List<PropMarker>();

        public static void Initialize()
        {
            markers = new Dictionary<Prop, PropMarker>();
            loadedMarkerObjects = new List<PropMarker>();

            container = new GameObject("[MonoDirector] - Prop Marker Container");
            container.transform.SetParent(Bootstrap.MainContainerObject.transform);

            for (int i = 0; i < 32; i++)
            {
                GameObject obj = GameObject.Instantiate(BundleLoader.PropMarkerObject);
                obj.transform.SetParent(container.transform);
                obj.transform.localPosition = Vector3.zero;

                PropMarker marker = new PropMarker();
                marker.SetMarker(obj);
                marker.SetProp(null);
                marker.Hide();

                loadedMarkerObjects.Add(marker);
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
        }

        public static void Update()
        {
            foreach (var marker in loadedMarkerObjects)
            {
                marker.Update();
            }
        }

        public static void AddMarkerToProp(Prop prop)
        {
            if (markers.ContainsKey(prop))
            {
                return;
            }

            PropMarker marker = loadedMarkerObjects.FirstOrDefault((marker) => !marker.Active);

            // HACK: Don't show prop markers during recording, if the prop was added during recording!
            if (Director.PlayState != PlayState.Recording)
            {
                marker.Show();
            }
            else
            {
                marker.Hide();
            }

            marker.SetOffset(Vector3.up * 0.125f);
            marker.SetProp(prop);
            markers.Add(prop, marker);
        }

        public static void RemoveMarkerFromProp(Prop prop)
        {
            if (!markers.ContainsKey(prop))
            {
                return;
            }

            PropMarker marker = markers[prop];
            marker.SetOffset(Vector3.zero);
            marker.SetProp(null);
            marker.Hide();
            markers.Remove(prop);
        }

        private static void ShowMarkers(PlayState playState)
        {
            if(playState == PlayState.Preplaying || playState == PlayState.Prerecording)
            {
                foreach (var marker in loadedMarkerObjects)
                {
                    marker.Hide();
                }
            }

            if(playState == PlayState.Stopped)
            {
                foreach (var marker in loadedMarkerObjects)
                {
                    if (marker.HasProp)
                    {
                        marker.Show();
                    }
                }
            }
        }
    }
}
