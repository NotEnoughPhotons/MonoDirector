using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.State;

using UnityEngine;

using BoneLib.BoneMenu;
using Il2CppSLZ.Marrow;
using Il2CppSystem;

namespace NEP.MonoDirector.UI
{
    internal static class MDBoneMenu
    {
        internal static Page rootCategory;

        internal static Page mdCategory;

        internal static Page playbackCategory;
        internal static Page actorCategory;
        internal static Page settingsCategory;

        internal static void Initialize()
        {
            rootCategory = Page.Root.CreatePage("Not Enough Photons", Color.white);

            mdCategory = rootCategory.CreatePage("Mono<color=red>Director</color>", Color.white);

            playbackCategory = mdCategory.CreatePage("Playback", Color.white);
            actorCategory = mdCategory.CreatePage("Actors", Color.white);
            settingsCategory = mdCategory.CreatePage("Settings", Color.white);

            BuildPlaybackMenu(playbackCategory);
            BuildActorMenu(actorCategory);
            BuildSettingsMenu(settingsCategory);
        }

        private static void BuildPlaybackMenu(Page category)
        {
            category.CreateFunction(
                "Record", 
                Color.red, 
                () => Director.Record()
            );
            
            category.CreateFunction(
                "Play", 
                Color.green, 
                () => Director.Play()
            );
            
            category.CreateFunction(
                "Stop", 
                Color.red, 
                () => Director.Stop()
            );
        }

        private static void BuildActorMenu(Page category)
        {
            category.CreateFunction(
                "Show Caster Menu", 
                Color.white,
                () => 
                {
                    MDMenu.Instance.gameObject.SetActive(true);
                }
            );
            
            category.CreateFunction(
                "Remove All Actors", 
                Color.red, 
                () =>
                {
                    Color primary = Color.red * 0.5f;
                    Color secondary = Color.red * 0.1f;

                    DialogData data = new()
                    {
                        Confirm = () => Director.RemoveAllActors(),
                        Message = "Are you sure? This cannot be undone.",
                        Primary = primary,
                        Secondary = secondary
                    };

                    Menu.DisplayDialog(data);
                    Director.RemoveAllActors();
                }
            );
            
            category.CreateFunction(
                "Clear Scene", 
                Color.red, 
                () =>
                {
                    Color primary = Color.red * 0.5f;
                    Color secondary = Color.red * 0.1f;

                    DialogData data = new()
                    {
                        Confirm = () => Director.ClearScene(),
                        Message = "Are you sure? This cannot be undone.",
                        Primary = primary,
                        Secondary = secondary
                    };

                    Menu.DisplayDialog(data);
                }
            );
        }

        private static void BuildSettingsMenu(Page category)
        {
            Page audioCategory = category.CreatePage("Audio", Color.white);
            Page cameraCategory = category.CreatePage("Camera", Color.white);
            Page toolCategory = category.CreatePage("Tools", Color.white);
            Page uiCategory = category.CreatePage("UI", Color.white);

            Page headModeCategory = cameraCategory.CreatePage("Head Mode Settings", Color.white);
            Page freeCamCategory = cameraCategory.CreatePage("Free Camera Settings", Color.white);
            Page vfxCategory = cameraCategory.CreatePage("VFX", Color.white);
            
            #if DEBUG
            Page debugCategory = category.CreatePage("DEBUG", Color.red);
            BuildDebugCategory(debugCategory);
            #endif

            audioCategory.CreateBool(
                "Use Microphone", 
                Color.white, 
                false,
                value => Settings.World.useMicrophone = value
            );
            
            audioCategory.CreateBool(
                "Mic Playback", 
                Color.white, 
                false,
                value => Settings.World.micPlayback = value
            );

            cameraCategory.CreateEnum(
                "Camera Mode", 
                Color.white, 
                CameraMode.None,
                (mode) => CameraRigManager.Instance.CameraMode = (CameraMode)mode
            );

            cameraCategory.CreateBool(
                "Kinematic On Release", 
                Color.white, 
                false,
                (value) => Settings.Camera.handheldKinematicOnRelease = value
            );

            BuildHeadModeCategory(headModeCategory);
            BuildFreeModeCategory(freeCamCategory);

            BuildVFXCategory(vfxCategory);

            BuildToolCategory(toolCategory);

            BuildUIMenu(uiCategory);
        }

        private static void BuildToolCategory(Page category)
        {
            category.CreateFloat(
                "Playback Speed",
                Color.white,
                1f,
                0.1f,
                float.NegativeInfinity,
                float.PositiveInfinity,
                value => Playback.Instance.PlaybackRate = value
            );

            category.CreateInt(
                "Delay",
                Color.white,
                5,
                1,
                0,
                30,
                value => Settings.World.delay = value
            );

            category.CreateInt(
                "FPS",
                Color.white,
                60,
                5,
                5,
                160,
                value => Settings.World.fps = value
            );

            category.CreateBool(
                "Ignore Slomo",
                Color.white,
                false,
                value => Settings.World.ignoreSlomo = value
            );

            category.CreateBool(
                "Temporal Scaling",
                Color.white,
                true,
                value => Settings.World.temporalScaling = value
            );
        }

        private static void BuildUIMenu(Page category)
        {
            category.CreateBool(
                "Show UI",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowUI = value
            );

            category.CreateBool(
                "Show Timecode",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowTimecode = value
            );

            category.CreateBool(
                "Show Play Mode",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowPlaymode = value
            );

            category.CreateBool(
                "Show Icons",
                Color.white,
                false,
                value => InformationInterface.Instance.ShowIcons = value
            );
        }

#if DEBUG
        private static void BuildDebugCategory(Page category)
        {
            category.CreateFunction(
                "Duplicate Player",
                Color.white,
                () =>
                {
                    RigManager rig = Constants.RigManager;
                    //rig.AvatarCrate.Crate.Spawn(rig.ControllerRig.m_head.position, Quaternion.identity);
                }
            );
            
            category.CreateBool(
                "Debug Mode", 
                Color.white, 
                false,
                value => Settings.Debug.debugEnabled = value
            );
            
            category.CreateBool(
                "Use Debug Keys", 
                Color.white, 
                false, 
                value => Settings.Debug.useKeys = value
            );
        }
#endif
        
        private static void BuildHeadModeCategory(Page headModeCategory)
        {
            headModeCategory.CreateFloat(
                "Interpolation", 
                Color.white, 
                4f, 
                1f, 
                0f, 
                64f,
                value => CameraRigManager.Instance.CameraSmoothness = value
            );

            headModeCategory.CreateEnum(
                "Position", 
                Color.white, 
                BodyPart.Head,
                bone => CameraRigManager.Instance.FollowCamera.SetFollowBone((BodyPart)bone)
            );
        }

        private static void BuildFreeModeCategory(Page freeModeCategory)
        {
            freeModeCategory.CreateFloat(
                "Mouse Sens.",
                Color.white,
                1f,
                0.5f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MouseSensitivity = value);

            freeModeCategory.CreateFloat(
                "Mouse Smoothing",
                Color.white,
                1f,
                0.5f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MouseSmoothness = value
            );

            freeModeCategory.CreateFloat(
                "Slow Speed",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.SlowSpeed = value
            );

            freeModeCategory.CreateFloat(
                "Fast Speed",
                Color.white,
                10f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.FastSpeed = value
            );

            freeModeCategory.CreateFloat(
                "Max Speed",
                Color.white,
                15f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.MaxSpeed = value
            );

            freeModeCategory.CreateFloat(
                "Friction",
                Color.white,
                5f,
                1f,
                0f,
                float.PositiveInfinity,
                (value) => CameraRigManager.Instance.Friction = value
            );
        }

        private static void BuildVFXCategory(Page vfxCategory)
        {
            vfxCategory.CreateBool(
                "Lens Distortion", 
                Color.white, 
                true,
                value => CameraRigManager.Instance.EnableLensDistortion(value)
            );
            
            //vfxCategory.CreateBool("Motion Blur", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MotionBlur.active = value);
            
            vfxCategory.CreateBool(
                "Chromatic Abberation", 
                Color.white, 
                true,
                value => CameraRigManager.Instance.EnableChromaticAbberation(value)
            );
            
            //vfxCategory.CreateBool("Vignette", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Vignette.active = true);

            //vfxCategory.CreateBool("Bloom", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.Bloom.active = true);
            //vfxCategory.CreateBool("MK Glow", Color.white, true, (value) => CameraRigManager.Instance.CameraVolume.MkGlow.active = true);
        }
    }
}