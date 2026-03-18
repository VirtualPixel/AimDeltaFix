using BepInEx;
using HarmonyLib;

namespace AimDeltaFix
{
    [BepInPlugin("Vippy.AimDeltaFix", "AimDeltaFix", "1.0.0")]
    public class AimDeltaFix : BaseUnityPlugin
    {
        internal Harmony? Harmony { get; set; }

        private void Awake()
        {
            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }
    }
}