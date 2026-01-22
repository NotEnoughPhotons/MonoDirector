using UnityEngine;

using BoneLib;

using MelonLoader.Utils;

using Il2CppSLZ.Marrow.Warehouse;

namespace NEP.MonoDirector.Data
{
    public static class WarehouseLoader
    {
        internal static Dictionary<string, AudioClip> soundTable;
        internal static List<AudioClip> sounds;

        internal static readonly string companyCode = "NotEnoughPhotons.";
        internal static readonly string modCode = "MonoDirector.";
        internal static readonly string typeCode = "Spawnable.";

        internal static readonly string propMarkerBarcode = CreateFullBarcode("PropMarker");
        internal static readonly string infoInterfaceBarcode = CreateFullBarcode("InformationInterface");
        internal static readonly string mainMenuBarcode = CreateFullBarcode("MonoDirectorMenu");

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

            Barcode mainBarcode = (Barcode)"NotEnoughPhotons.MonoDirector";
            if (!AssetWarehouse.Instance.HasPallet(mainBarcode))
            {
                Main.Logger.Error("Pallet doesn't exist in registry.");
                return;
            }

            PalletManifest palletManifest = AssetWarehouse.Instance.palletManifests[mainBarcode];

            if (palletManifest == null)
            {
                Main.Logger.Error("Pallet manifest is null.");
                return;
            }

            Pallet pallet = palletManifest.Pallet;

            SpawnableCrate spawnable = null;
            foreach (Crate crate in pallet.Crates)
            {
                Main.Logger.Msg(crate.Barcode);
                if (crate.Barcode == (Barcode)CreateFullBarcode("SoundHolder"))
                {
                    spawnable = crate.Cast<SpawnableCrate>();
                    break;
                }
            }

            if (spawnable == null)
            {
                Main.Logger.Error("Sound holder spawnable is null.");
                return;
            }

            foreach (var sound in sounds)
            {
                SpawnableCrate copyCrate = new SpawnableCrate()
                {
                    Title = $"SFX - {sound.name}",
                    Barcode = (Barcode)$"NotEnoughPhotons.MonoDirector.Spawnables.SFX{sound.name}",
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

        internal static GameObject SpawnFromBarcode(string barcode, bool active = false)
        {
            GameObject spawnedObject = null;
            HelperMethods.SpawnCrate(barcode, Vector3.zero, Quaternion.identity, Vector3.one, false, (obj) =>
            {
                spawnedObject = obj;
                spawnedObject.SetActive(active);
            });

            return spawnedObject;
        }

        internal static List<GameObject> SpawnFromBarcode(string barcode, int amount, bool active = false)
        {
            List<GameObject> spawnedObjects = new List<GameObject>();

            for (int i = 0; i < amount; i++)
            {
                var obj = SpawnFromBarcode(barcode, active);
                obj.SetActive(active);
                spawnedObjects.Add(obj);
            }

            return spawnedObjects;
        }

        private static string CreateFullBarcode(string spawnableName)
        {
            return companyCode + modCode + typeCode + spawnableName;
        }
    }
}
