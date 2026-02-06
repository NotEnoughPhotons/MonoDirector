using UnityEngine;

using BoneLib;

using MelonLoader.Utils;

using Il2CppSLZ.Marrow.Warehouse;

using NEP.MonoDirector.Core;

namespace NEP.MonoDirector.Data
{
    public static class WarehouseLoader
    {
        internal static Dictionary<string, AudioClip> soundTable;
        internal static List<AudioClip> sounds;

        internal static readonly string companyCode = "NEP.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly Barcode propMarkerBarcode = CreateFullBarcode("PropMarker");
        internal static readonly Barcode infoInterfaceBarcode = CreateFullBarcode("InformationInterface");
        internal static readonly Barcode mainMenuBarcode = CreateFullBarcode("MonoDirectorMenu");

        internal static List<AudioClip> GetSounds()
        {
            List<AudioClip> sounds = new List<AudioClip>();
            soundTable = new Dictionary<string, AudioClip>();
            string path = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons/MonoDirector/SFX/Sounds");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            IEnumerable<string> files = Directory.EnumerateFiles(path);

            foreach (var file in files)
            {
                var clip = AudioImportLib.API.LoadAudioClip(file, true);
                clip.hideFlags = HideFlags.DontUnloadUnusedAsset;
                sounds.Add(clip);
            }

            return sounds;
        }

        internal static void GenerateSpawnablesFromSounds(AudioClip[] sounds)
        {
            if (sounds == null)
            {
                return;
            }

            if (sounds.Length == 0)
            {
                return;
            }

            Barcode mainBarcode = new Barcode("NEP.MonoDirector");
            if (!AssetWarehouse.Instance.HasPallet(mainBarcode))
            {
                Logging.Error("Pallet doesn't exist in registry.");
                return;
            }

            PalletManifest palletManifest = AssetWarehouse.Instance.palletManifests[mainBarcode];

            if (palletManifest == null)
            {
                Logging.Error("Pallet manifest is null.");
                return;
            }
            
            Pallet pallet = palletManifest.Pallet;

            SpawnableCrate spawnable = null;
            foreach (Crate crate in pallet.Crates)
            {
                Logging.Msg(crate.Barcode.ID);
                if (crate.Barcode == CreateFullBarcode("SoundHolder"))
                {
                    spawnable = crate.Cast<SpawnableCrate>();
                    break;
                }
            }

            if (spawnable == null)
            {
                Logging.Error("Sound holder spawnable is null.");
                return;
            }

            foreach (var sound in sounds)
            {
                SpawnableCrate copyCrate = new SpawnableCrate()
                {
                    Title = $"SFX - {sound.name}",
                    Barcode = new Barcode($"NEP.MonoDirector.Spawnables.SFX{sound.name}"),
                    Description = sound.name,
                    Pallet = spawnable.Pallet,
                    _packedAssets = spawnable.PackedAssets,
                    MainAsset = spawnable.MainAsset,
                    MainGameObject = spawnable.MainGameObject
                };

                copyCrate.name = copyCrate.Title;
                soundTable.Add(copyCrate.Description, sound);
                pallet.Crates.Add(copyCrate);
                AssetWarehouse.Instance.AddCrate(copyCrate);
            }
        }

        internal static GameObject SpawnFromBarcode(Barcode barcode, bool active = false)
        {
            GameObject spawnedObject = null;
            HelperMethods.SpawnCrate(barcode.ID, Vector3.zero, Quaternion.identity, Vector3.one, false, (obj) =>
            {
                spawnedObject = obj;
                spawnedObject.SetActive(active);
            });

            return spawnedObject;
        }

        internal static List<GameObject> SpawnFromBarcode(Barcode barcode, int amount, bool active = false)
        {
            List<GameObject> spawnedObjects = new List<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                var obj = SpawnFromBarcode(new Barcode(barcode), active);
                obj.SetActive(active);
                spawnedObjects.Add(obj);
            }

            return spawnedObjects;
        }

        private static Barcode CreateFullBarcode(string spawnableName)
        {
            return new Barcode(companyCode + modCode + typeCode + spawnableName);
        }
    }
}
