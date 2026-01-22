using System;
using System.Diagnostics;
using System.Reflection;
using System.IO;

using MelonLoader;
using UnityEngine;

using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Cameras;
using NEP.MonoDirector.Core;
using NEP.MonoDirector.UI;
using NEP.MonoDirector.State;

using BoneLib.BoneMenu;
using NEP.MonoDirector.Data;
using Il2CppSLZ.Marrow.Warehouse;
using Harmony;

namespace NEP.MonoDirector
{
    public static partial class BuildInfo
    {
        public const string Name = "MonoDirector"; // Name of the Mod.  (MUST BE SET)
        public const string Description = "A movie/photo making utility for BONELAB!"; // Description for the Mod.  (Set as null if none)
        public const string Author = "Not Enough Photons"; // Author of the Mod.  (MUST BE SET)
        public const string Company = null; // Company that made the Mod.  (Set as null if none)
        public const string Version = "0.0.1"; // Version of the Mod.  (MUST BE SET)
        public const string DownloadLink = null; // Download Link for the Mod.  (Set as null if none)
    }

    public class Main : MelonMod
    {
        internal static MelonLogger.Instance Logger;

        public static Main instance;

        public static Director director;

        public static FreeCamera camera;

        public static FeedbackSFX feedbackSFX;
        public static MDMenu mainMenu;

        public static AssetBundle bundle;

        public override void OnInitializeMelon()
        {
            Logger = new MelonLogger.Instance("MonoDirector", System.Drawing.Color.Magenta);

            instance = this;

            Directory.CreateDirectory(Constants.dirBase);
            Directory.CreateDirectory(Constants.dirMod);
            Directory.CreateDirectory(Constants.dirSFX);
            Directory.CreateDirectory(Constants.dirImg);

            bundle = GetEmbeddedBundle();

            BoneLib.Hooking.OnLevelLoaded += (info) => MonoDirectorInitialize();
            BoneLib.Hooking.OnWarehouseReady += new System.Action(() =>
            {
                AudioClip[] sounds = WarehouseLoader.GetSounds().ToArray();
                WarehouseLoader.GenerateSpawnablesFromSounds(sounds);
            });

            MDBoneMenu.Initialize();
#if DEBUG
            Logger.Warning("MONODIRECTOR DEBUG BUILD!");

            // Testing
            Logger.Warning("Writing test frame data, better hope this doesn't violently crash!!!");
            Data.ObjectFrame frame = new Data.ObjectFrame();
            frame.frameTime = 3.141592654F;
            frame.position = new Vector3(1, -1, 2);
            frame.rotation = new Quaternion(0.5F, 0.75F, 0.9F, 1.0F).normalized;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            byte[] frameBytes = frame.ToBinary();
            sw.Stop();
            
            Logger.Msg($"[STOPWATCH]: ToBinary() took {sw.ElapsedMilliseconds}...");

            sw.Restart();
            
            using (FileStream file = File.Open("test.mdbf", FileMode.Create))
            {
                uint ident = frame.GetBinaryID();
                file.Write(BitConverter.GetBytes(ident), 0, sizeof(uint));
                
                file.Write(frameBytes, 0, frameBytes.Length);
            };
            
            sw.Stop();
            
            Logger.Msg($"[STOPWATCH]: Writing MDBF took {sw.ElapsedMilliseconds}...");
            
            sw.Restart();
            
            // Then try to read it back
            using (FileStream file = File.Open("test.mdbf", FileMode.Open))
            {
                // Seek past the first 4 bytes
                file.Seek(4, SeekOrigin.Begin);
                frame.FromBinary(file);
            }

            sw.Stop();

            Logger.Msg($"[STOPWATCH]: FromBinary() took {sw.ElapsedMilliseconds}...");

            Logger.Msg("READING...");
            Logger.Msg($"\tFrT = {frame.frameTime}");
            Logger.Msg($"\tPos = {frame.position}");
            Logger.Msg($"\tRot = {frame.rotation}");
#endif
        }

        private void MonoDirectorInitialize()
        {
            ResetInstances();
            CreateCameraManager();
            CreateDirector();
            CreateSFX();
            CreateUI();

            // Data.AvatarPhotoBuilder.Initialize();
        }

        private void ResetInstances()
        {
            Events.FlushActions();
            director = null;
            camera = null;
            feedbackSFX = null;
            PropMarkerManager.CleanUp();
        }

        private void CreateCameraManager()
        {
            new CameraRigManager();
        }

        private void CreateDirector()
        {
            GameObject directorObject = new GameObject("MonoDirector - Director");
            director = directorObject.AddComponent<Director>();
            director.SetCamera(camera);
        }

        private void CreateSFX()
        {
            GameObject audioManager = new GameObject("MonoDirector - Audio Manager");
            audioManager.AddComponent<AudioManager>();

            GameObject feedback = new GameObject("MonoDirector - Feedback SFX");
            feedbackSFX = feedback.AddComponent<FeedbackSFX>();
        }

        private void CreateUI()
        {
            // PropMarkerManager.Initialize();
            InfoInterfaceManager.Initialize();
            WarehouseLoader.SpawnFromBarcode(WarehouseLoader.mainMenuBarcode);
        }

        private static AssetBundle GetEmbeddedBundle()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            string fileName = "md_resources.pack";

            using (Stream resourceStream = assembly.GetManifestResourceStream("NEP.MonoDirector.Resources." + fileName))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    resourceStream.CopyTo(memoryStream);
                    return AssetBundle.LoadFromMemory(memoryStream.ToArray());
                }
            }
        }
    }
}
