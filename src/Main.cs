using MelonLoader;
using NEP.MonoDirector.Core;

namespace NEP.MonoDirector
{
    public static partial class BuildInfo
    {
        public const string Name = "MonoDirector";
        public const string Description = "A movie/photo making utility for BONELAB!";
        public const string Author = "Not Enough Photons";
        public const string Company = "Not Enough Photons";
        public const string Version = "1.2.0";
        public const string DownloadLink = "https://bonelab.thunderstore.io/c/bonelab/p/NotEnoughPhotons/MonoDirector";
    }

    public class Main : MelonMod
    {
        public override void OnInitializeMelon() => Bootstrap.Initialize();

        public override void OnDeinitializeMelon() => Bootstrap.Shutdown();
    }
}
