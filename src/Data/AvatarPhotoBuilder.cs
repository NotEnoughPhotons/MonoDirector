using UnityEngine.Rendering.Universal;
using UnityEngine;

using Il2CppInterop.Runtime;

using Il2CppSLZ.Marrow.Warehouse;

using MarrowAvatar = Il2CppSLZ.VRMK.Avatar;

namespace NEP.MonoDirector.Data
{
    public static class AvatarPhotoBuilder
    {
        public static Dictionary<string, Texture2D> avatarPortraits = new Dictionary<string, Texture2D>();

        private static bool initialized = false;

        private static GameObject renderCameraObject;
        private static Camera renderCamera;
        private static Transform renderCameraTransform;

        public static void Initialize()
        {
            if (initialized)
            {
                return;
            }

            renderCameraObject = new GameObject("Avatar Render Camera");
            renderCamera = renderCameraObject.AddComponent<Camera>();
            renderCameraObject.AddComponent<UniversalAdditionalCameraData>().allowXRRendering = false;
            renderCameraTransform = renderCameraObject.transform;

            var pallets = AssetWarehouse.Instance.GetPallets();

            foreach (var pallet in pallets)
            {
                foreach (var crate in pallet.Crates)
                {
                    if (crate.GetIl2CppType() != Il2CppType.Of<AvatarCrate>())
                    {
                        continue;
                    }

                    string imageDirectory = Constants.dirImg;
                    string crateTitle = crate.Title;
                    string portraitFile = Path.Combine(imageDirectory, crateTitle + ".png");

                    if (File.Exists(portraitFile))
                    {
                        Texture2D portrait = new Texture2D(2048, 2048);
                        portrait.hideFlags = HideFlags.DontUnloadUnusedAsset;
                        byte[] imageData = File.ReadAllBytes(portraitFile);
                        ImageConversion.LoadImage(portrait, imageData);

                        if (!avatarPortraits.ContainsKey(crateTitle))
                        {
                            avatarPortraits.Add(crateTitle, portrait);
                        }
                    }
                    else
                    {
                        crate.LoadAsset(new Action<UnityEngine.Object>((obj) => CreateDummyAvatarObject(obj, crateTitle)));
                    }
                }
            }

            GameObject.Destroy(renderCameraObject);

            initialized = true;
        }

        private static void CreateDummyAvatarObject(UnityEngine.Object obj, string crateTitle)
        {
            GameObject avatarInstance = GameObject.FindObjectFromInstanceID(obj.GetInstanceID()).Cast<GameObject>();
            avatarInstance.transform.position = Vector3.zero;
            Texture2D icon = CreateAvatarIcon(avatarInstance);
            icon.hideFlags = HideFlags.DontUnloadUnusedAsset;

            byte[] imageBytes = ImageConversion.EncodeToPNG(icon);
            File.WriteAllBytes(Path.Combine(Constants.dirImg, $"{crateTitle}.png"), imageBytes);

            if (!avatarPortraits.ContainsKey(crateTitle))
            {
                avatarPortraits.Add(crateTitle, icon);
            }
        }

        private static Texture2D CreateAvatarIcon(GameObject avatarObject)
        {
            GameObject copiedAvatar = GameObject.Instantiate(avatarObject);

            MarrowAvatar avatar = copiedAvatar.GetComponent<MarrowAvatar>();

            RenderTexture input = new RenderTexture(256, 256, -1, RenderTextureFormat.ARGB32);

            Texture2D output = new Texture2D(256, 256);

            renderCamera.targetTexture = input;

            Transform head = avatar.animator.GetBoneTransform(HumanBodyBones.Head);
            renderCameraTransform.position = avatar.transform.forward + Vector3.up * avatar.height * 0.95f;
            renderCameraTransform.rotation = Quaternion.LookRotation(head.position - renderCameraTransform.position);

            renderCamera.Render();

            RenderTexture.active = input;

            output.ReadPixels(new Rect(0f, 0f, output.width, output.height), 0, 0);
            output.Apply();

            RenderTexture.active = null;

            GameObject.Destroy(copiedAvatar);
            GameObject.Destroy(renderCameraObject);

            return output;
        }
    }
}
