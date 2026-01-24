using System.Reflection;
using MelonLoader;

[assembly: AssemblyTitle(NEP.MonoDirector.BuildInfo.Name)]
[assembly: AssemblyDescription(NEP.MonoDirector.BuildInfo.Description)]
[assembly: AssemblyCompany(NEP.MonoDirector.BuildInfo.Company)]
[assembly: AssemblyProduct(NEP.MonoDirector.BuildInfo.Name)]
[assembly: AssemblyCopyright("Developed by " + NEP.MonoDirector.BuildInfo.Author)]
[assembly: AssemblyTrademark(NEP.MonoDirector.BuildInfo.Company)]
[assembly: AssemblyVersion(NEP.MonoDirector.BuildInfo.Version)]
[assembly: AssemblyFileVersion(NEP.MonoDirector.BuildInfo.Version)]
[assembly: MelonInfo(typeof(NEP.MonoDirector.Main), NEP.MonoDirector.BuildInfo.Name, NEP.MonoDirector.BuildInfo.Version, NEP.MonoDirector.BuildInfo.Author, NEP.MonoDirector.BuildInfo.DownloadLink)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]