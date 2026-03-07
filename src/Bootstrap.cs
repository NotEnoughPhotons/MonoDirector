using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using BoneLib;

using NEP.MonoDirector.UI;
using NEP.MonoDirector.Data;
using NEP.MonoDirector.Audio;
using NEP.MonoDirector.Downloading;
using MelonLoader;
using Il2CppSLZ.Marrow.Warehouse;
using BoneLib.Notifications;
using System.Linq.Expressions;

namespace NEP.MonoDirector.Core
{
    public static class Bootstrap
    {
        internal static GameObject MainContainerObject { get; private set; }

        internal static bool AudioImportInstalled { get => m_audioImportInstalled; }

        private static bool m_audioImportInstalled = false;

        internal static void Initialize()
        {
            Logging.Initialize();

            Directory.CreateDirectory(Constants.dirBase);
            Directory.CreateDirectory(Constants.dirMod);
            Directory.CreateDirectory(Constants.dirSFX);
            Directory.CreateDirectory(Constants.dirImg);

            Hooking.OnLevelLoaded += (info) => OnLevelLoaded();
            Hooking.OnWarehouseReady += OnWarehouseReady;

            MDBoneMenu.Initialize();

            CheckAudioImport();

            if (m_audioImportInstalled)
            {
                FeedbackSFX.Initialize();
            }

            BundleLoader.Initialize();
#if DEBUG
            TestDebugSerialize();
#endif
        }

        internal static void Shutdown()
        {
            FeedbackSFX.Shutdown();
        }

        internal static void OnWarehouseReady()
        {
            AssetWarehouse.Instance.OnPalletAdded += new Action<Barcode>(OnPalletAdded);

            if (!m_audioImportInstalled)
            {
                Logging.Warn("AudioImportLib is not installed!");
                return;
            }

            if (AssetDownloader.CheckInstall())
            {
                WarehouseLoader.LoadSounds();
                WarehouseLoader.GenerateSpawnablesFromSounds();
            }
        }

        internal static void OnPalletAdded(Barcode barcode)
        {
            if (barcode.ID != "NEP.MonoDirector")
            {
                return;
            }

            if (!m_audioImportInstalled)
            {
                Notification notification = new()
                {
                    Title = "Missing AudioImportLib",
                    Message = "You do not have AudioImportLib installed! Download it from Thunderstore and install it!",
                    Type = NotificationType.Warning,
                    PopupLength = 5f
                };

                Notifier.Send(notification);

                return;
            }

            WarehouseLoader.LoadSounds();
            WarehouseLoader.GenerateSpawnablesFromSounds();
        }

        internal static void OnLevelLoaded()
        {
            if (!AssetDownloader.CheckInstall())
            {
                Notification notification = new()
                {
                    Title = "Missing Content Pallet",
                    Message = "You do not have the MonoDirector content pallet installed! Subscribe to it on mod.io, then install it in game!",
                    Type = NotificationType.Warning,
                    PopupLength = 5f
                };

                Notifier.Send(notification);
            }

            if (!m_audioImportInstalled)
            {
                Notification notification = new()
                {
                    Title = "Missing AudioImportLib",
                    Message = "You do not have AudioImportLib installed! Download it from Thunderstore and install it!",
                    Type = NotificationType.Warning,
                    PopupLength = 5f
                };

                Notifier.Send(notification);
            }

            MainContainerObject = new GameObject("[MonoDirector]");

            Events.FlushActions();

            if (m_audioImportInstalled)
            {
                FeedbackSFX.Initialize();
            }

            Director.Shutdown();
            Director.Initialize();

            CreateUI();
        }

        internal static void CreateUI()
        {
            PropMarkerManager.Initialize();
            InfoInterfaceManager.Initialize();
            ActorFrameManager.Initialize();
            WarehouseLoader.SpawnFromBarcode(WarehouseLoader.actorPanelBarcode);
            WarehouseLoader.SpawnFromBarcode(WarehouseLoader.mainMenuBarcode);
        }

        private static void CheckAudioImport()
        {
            if (MelonBase.FindMelon("AudioImportLib", "trev") != null)
            {
                m_audioImportInstalled = true;
            }
        }

        internal static void TestDebugSerialize()
        {
            Logging.Warn("MONODIRECTOR DEBUG BUILD!");

            // Testing
            Logging.Warn("Writing test frame data, better hope this doesn't violently crash!!!");
            Data.ObjectFrame frame = new Data.ObjectFrame();
            frame.frameTime = 3.141592654F;
            frame.position = new Vector3(1, -1, 2);
            frame.rotation = new Quaternion(0.5F, 0.75F, 0.9F, 1.0F).normalized;

            Stopwatch sw = new Stopwatch();
            sw.Start();
            byte[] frameBytes = frame.ToBinary();
            sw.Stop();

            Logging.Msg($"[STOPWATCH]: ToBinary() took {sw.ElapsedMilliseconds}...");

            sw.Restart();

            using (FileStream file = File.Open("test.mdbf", FileMode.Create))
            {
                uint ident = frame.GetBinaryID();
                file.Write(BitConverter.GetBytes(ident), 0, sizeof(uint));

                file.Write(frameBytes, 0, frameBytes.Length);
            }
            ;

            sw.Stop();

            Logging.Msg($"[STOPWATCH]: Writing MDBF took {sw.ElapsedMilliseconds}...");

            sw.Restart();

            // Then try to read it back
            using (FileStream file = File.Open("test.mdbf", FileMode.Open))
            {
                // Seek past the first 4 bytes
                file.Seek(4, SeekOrigin.Begin);
                frame.FromBinary(file);
            }

            sw.Stop();

            Logging.Msg($"[STOPWATCH]: FromBinary() took {sw.ElapsedMilliseconds}...");

            Logging.Msg("READING...");
            Logging.Msg($"\tFrT = {frame.frameTime}");
            Logging.Msg($"\tPos = {frame.position}");
            Logging.Msg($"\tRot = {frame.rotation}");
        }
    }
}
