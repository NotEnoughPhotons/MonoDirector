using Il2CppSystem;

using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSLZ.Bonelab;
using Il2CppSLZ.Marrow;
using Il2CppSLZ.Marrow.Interaction;
using Il2CppSLZ.Marrow.Warehouse;

using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NEP.MonoDirector.Actors
{
    public sealed class ActorRig
    {
        // Special thanks to the BONELAB Fusion repo by Lakatrazz
        // https://github.com/Lakatrazz/BONELAB-Fusion/blob/main/LabFusion/src/Representation/PlayerRepUtilities.cs
        
        internal static string RepName = "[RigManager (MonoDirector)]";
        
        private float _health;
        private bool _ragdoll;
        private bool _currentlyRagdolled;
        
        public static void CreateRig(Action<RigManager> onRigCreated)
        {
            if (MarrowSettings.RuntimeInstance == null)
            {
                return;
            }

            var crate = MarrowSettings.RuntimeInstance.DefaultPlayerRig.Crate;
            if (crate == null)
            {
                return;
            }

            crate.LoadAsset((Action<GameObject>)((go) => LoadRig(go, onRigCreated)));
        }

        private static void LoadRig(GameObject rigObject, Action<RigManager> onRigCreated)
        {
            GameObject temp = new();
            temp.SetActive(false);
            
            GameObject rigAsset = rigObject.GetComponentInChildren<RigManager>().gameObject;

            if (!rigAsset)
            {
                return;
            }

            GameObject rigClone = GameObject.Instantiate(rigAsset, temp.transform);
            rigClone.name = RepName;
            rigClone.SetActive(false);

            Vector3 test = new Vector3(0f, 2.0f, 0.0f);

            rigClone.transform.position = test;
            rigClone.transform.rotation = Quaternion.identity;
            
            RigManager rigManager = rigClone.GetComponent<RigManager>();

            SetupRig(rigManager);
            
            rigClone.transform.SetParent(null);
            GameObject.Destroy(temp);
            
            rigClone.SetActive(true);
            
            onRigCreated?.Invoke(rigManager);
        }

        private static void SetupRig(RigManager rigManager)
        {
            OpenControllerRig openControllerRig = rigManager.ControllerRig.TryCast<OpenControllerRig>();
            MarrowEntity entity = rigManager.physicsRig.marrowEntity;

            BoneTagReference monoTag = new("NotEnoughPhotons.BoneTag.Actor");
            entity.Tags.Tags.Add(monoTag);
            
            MarrowEntity controllerEntity = openControllerRig.GetComponent<MarrowEntity>();
            MarrowBody[] controllerBodies = openControllerRig.GetComponentsInChildren<MarrowBody>();
            Tracker[] controllerTrackers = openControllerRig.GetComponentsInChildren<Tracker>();

            foreach (var body in controllerBodies)
            {
                GameObject.DestroyImmediate(body);
            }

            foreach (var tracker in controllerTrackers)
            {
                GameObject.DestroyImmediate(tracker.gameObject);
            }
            
            GameObject.DestroyImmediate(controllerEntity);
            
            Player_Health health = rigManager.health.TryCast<Player_Health>();

            if (health)
            {
                health.reloadLevelOnDeath = false;
                health.healthMode = Health.HealthMode.Invincible;

                var newVignetter = GameObject.Instantiate(health.Vignetter);
                newVignetter.GetComponent<SkinnedMeshRenderer>().enabled = false;
                newVignetter.name = "Vignetter";
                newVignetter.SetActive(false);
                
                health.Vignetter = newVignetter;
            }

            health._testVisualDamage = true;

            openControllerRig.quickmenuEnabled = false;
            openControllerRig._timeInput = false;
            rigManager.remapHeptaRig.doubleJump = false;

            var headset = openControllerRig.headset;
            GameObject.DestroyImmediate(headset.GetComponent<AudioListener>());
            GameObject.DestroyImmediate(headset.GetComponent<CameraSettings>());
            GameObject.DestroyImmediate(headset.GetComponent<StreamingController>());
            GameObject.DestroyImmediate(headset.GetComponent<VolumetricRendering>());
            GameObject.DestroyImmediate(headset.GetComponent<UniversalAdditionalCameraData>());
            GameObject.DestroyImmediate(headset.GetComponent<Camera>());

            openControllerRig.cameras = new Il2CppReferenceArray<Camera>(0);
            openControllerRig.onLastCameraUpdate = null;

            headset.tag = "Untagged";
            
            GameObject.DestroyImmediate(rigManager.GetComponentInChildren<PlayerAvatarArt>(true));
            
            GameObject.DestroyImmediate(rigManager.ControllerRig.leftController.GetComponent<UIControllerInput>());
            GameObject.DestroyImmediate(rigManager.ControllerRig.rightController.GetComponent<UIControllerInput>());

            SetupImpactProperties(rigManager);
        }

        private static void SetupImpactProperties(RigManager rigManager)
        {
            MarrowEntity entity = rigManager.GetComponent<MarrowEntity>();
            MarrowBody[] bodies = entity.Bodies;
            DataCardReference<SurfaceDataCard> bloodCard = new("SLZ.Backlot.SurfaceDataCard.Blood");
            
            foreach (MarrowBody body in bodies)
            {
                if (!body.HasRigidbody)
                {
                    continue;
                }
                
                ImpactProperties properties = body.GetComponent<ImpactProperties>();

                if (!properties)
                {
                    continue;
                }

                properties.SurfaceDataCard = bloodCard;
                properties.decalType = ImpactProperties.DecalType.None;
            }
        }
    }
}
