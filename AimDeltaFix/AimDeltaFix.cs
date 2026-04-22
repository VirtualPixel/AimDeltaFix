using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace AimDeltaFix
{
    [BepInPlugin("Vippy.AimDeltaFix", "AimDeltaFix", "1.0.3")]
    public class AimDeltaFix : BaseUnityPlugin
    {
        internal Harmony? Harmony { get; set; }

        private void Awake()
        {
            if (GameHasUpstreamFix())
            {
                Logger.LogInfo($"Skipping patch; REPO {Application.version} has the fix upstream");
                return;
            }

            Patch();

            Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
        }

        internal void Patch()
        {
            Harmony ??= new Harmony(Info.Metadata.GUID);
            Harmony.PatchAll();
        }

        // Upstream fix lands past 0.3.2. Disable to avoid double-patching once players update.
        private static bool GameHasUpstreamFix()
        {
            try
            {
                var raw = Application.version ?? "";
                var head = raw.Split('-')[0].Trim();
                return System.Version.Parse(head) > new System.Version(0, 3, 2);
            }
            catch
            {
                return false;
            }
        }
    }
}