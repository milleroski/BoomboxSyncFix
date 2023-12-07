using BepInEx;
using BepInEx.Logging;
using BoomboxSyncFix.Patches;
using HarmonyLib;

namespace BoomboxSyncFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class BoomboxSyncFixPlugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static BoomboxSyncFixPlugin Instance;

        internal ManualLogSource logger;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            logger.LogInfo("Plugin BoomboxSyncFix has loaded!");

            harmony.PatchAll(typeof(BoomboxSyncFixPlugin));
            harmony.PatchAll(typeof(BoomboxItemStartMusicPatch));
        }
    }
}