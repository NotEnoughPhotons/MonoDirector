using MelonLoader;
using Il2CppSLZ.Marrow;

using System.IO;
using MelonLoader.Utils;

namespace NEP.MonoDirector
{
    public static class Constants
    {
        public static RigManager RigManager => BoneLib.Player.RigManager;

        public static readonly string dirBase = Path.Combine(MelonEnvironment.UserDataDirectory, "Not Enough Photons");
        public static readonly string dirMod = Path.Combine(dirBase, "MonoDirector");
        public static readonly string dirImg = Path.Combine(dirMod, "Images/");
        public static readonly string dirSFX = Path.Combine(dirMod, "SFX/");
    }
}
