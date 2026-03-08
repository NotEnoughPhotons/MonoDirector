using BoneLib;
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
            // if (CheckInstall())
            // {
            //     yield break;
            // }

            var task = FetchModFileAsync();

            while (!task.IsCompleted) yield return null;
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

        private static async Task FetchModFileAsync()
        {
            try
            {
                Uri uri = new Uri($"https://g-{GameID}.modapi.io/v1/games/{GameID}/mods/{ModID}/files/?api_key={APIKey}");

                HttpClient client = null;
                
                // B.S. way to get around SSL validation not working on Quest.
                // This might be a security issue.
                if (HelperMethods.IsAndroid())
                {
                    // It is absolutely not recommended to use this code.
                    // Bypassing SSL certificates and validation altogether is a massive security breach waiting to happen.
                    // BUT...
                    // The request URI will never change, and it is only done here once, and exists only for the Quest platform.
                    HttpClientHandler handler = new HttpClientHandler();
                    handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                    client = new HttpClient(handler);
                }
                else
                {
                    client = new HttpClient();
                }

                var request = new HttpRequestMessage(HttpMethod.Get, uri);

                request.Headers.Add("Accept", "application/json");

                var response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode();

                string content = await response.Content.ReadAsStringAsync();

                client = new HttpClient();

                DataResult result = JsonConvert.DeserializeObject<DataResult>(content);
                m_modFile = result.Data[0];
            }
            catch (System.Exception e)
            {
                Logging.Error($"Exception caught! {e.StackTrace} - {e.Message}");
            }
        }
    }
}
