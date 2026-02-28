using Il2CppSLZ.Marrow.Forklift;
using Il2CppSLZ.Marrow.Forklift.Model;
using Il2CppSLZ.Marrow.Warehouse;
using MelonLoader;
using NEP.MonoDirector.Core;

using Newtonsoft.Json;
using System.Collections;
using System.IO.Compression;

using UnityEngine;
using UnityEngine.Playables;

namespace NEP.MonoDirector.Downloading
{
    public static class AssetDownloader
    {
        private static bool m_installed = false;

        private static string m_modsPath = Path.Combine(Application.persistentDataPath, "Mods");

        private static ModObject m_modFile;

        private static readonly int GameID = 3809;
        private static readonly int ModID = 4249462;
        private static readonly string APIKey = "c6822901d2fd4fa27e8dfc9c34488ba9";

        public static IEnumerator Start()
        {
            if (CheckInstall())
            {
                yield break;
            }

            yield return DownloadRoutine();
        }

        public static bool NeedsNewVersion()
        {
            if (!AssetWarehouse.Instance.TryGetPallet(new Barcode("NEP.MonoDirector"), out Pallet pallet))
            {
                return false;
            }

            string[] versions = pallet.Version.Split('.');
            string[] websiteVersions = m_modFile.Version.Split(".");

            PalletVersion version = new(int.Parse(versions[0]), int.Parse(versions[1]), int.Parse(versions[2]));
            PalletVersion websiteVersion = new(int.Parse(websiteVersions[0]), int.Parse(websiteVersions[1]), int.Parse(websiteVersions[2]));

            return version != websiteVersion;
        }

        public static bool CheckInstall()
        {
            string modsLocation = Path.Combine(m_modsPath, "NEP.MonoDirector");

            if (!Directory.Exists(modsLocation))
            {
                return false;
            }

            return true;
        }

        private static IEnumerator DownloadRoutine()
        {
            var fetchTask = FetchModFileAsync();

            while (!fetchTask.IsCompleted) yield return null;

            var downloadTask = BeginDownloadAsync();

            while (!downloadTask.IsCompleted) yield return null;

            var warehouseTask = AssetWarehouse.Instance.LoadPalletsFromFolderAsync(m_modsPath).GetAwaiter();

            while (!warehouseTask.IsCompleted) yield return null;

            yield return null;
        }

        private static async Task FetchModFileAsync()
        {
            try
            {
                Uri uri = new Uri($"https://g-{GameID}.modapi.io/v1/games/{GameID}/mods/{ModID}?api_key={APIKey}");

                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.Add("Accept", "application/json");

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                client = new HttpClient();

                m_modFile = JsonConvert.DeserializeObject<ModObject>(content);
            }
            catch (System.Exception e)
            {
                Logging.Error($"Exception caught! {e.StackTrace} - {e.Message}");
            }
        }

        private static async Task BeginDownloadAsync()
        {
            try
            {
                HttpClient client = new HttpClient();

                PlatformObject windowsPlatform = m_modFile.Platforms[0];

                Uri modUri = new Uri($"https://g-{GameID}.modapi.io/v1/games/{GameID}/mods/{ModID}/files/{windowsPlatform.FileID}/download");

                using (var downloadStream = client.GetStreamAsync(modUri))
                {
                    using (var fileStream = new FileStream(Path.Combine(m_modsPath, "md-temp.zip"), FileMode.OpenOrCreate))
                    {
                        Logging.Msg("Downloading file...");
                        await downloadStream.Result.CopyToAsync(fileStream);
                        Logging.Msg("Unzipping file...");
                        ZipArchive archive = new ZipArchive(fileStream);
                        archive.ExtractToDirectory(m_modsPath, true);
                        archive.Dispose();
                        Logging.Msg("Done!");
                    }

                    File.Delete(Path.Combine(m_modsPath, "md-temp.zip"));
                }
            }
            catch (Exception e)
            {
                Logging.Error($"Exception caught! {e.StackTrace} - {e.Message}");
            }
        }
    }
}
