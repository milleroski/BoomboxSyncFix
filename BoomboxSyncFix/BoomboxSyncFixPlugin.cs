using BepInEx;
using BepInEx.Logging;
using BoomboxSyncFix.Networking;
using BoomboxSyncFix.Patches;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BoomboxSyncFix
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class BoomboxSyncFixPlugin : BaseUnityPlugin
    {
        private readonly Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);

        public static BoomboxSyncFixPlugin Instance;

        internal ManualLogSource logger;

        private static void NetcodeWeaver()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            NetcodeWeaver();

            logger = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            logger.LogInfo("Plugin BoomboxSyncFix has loaded!");

            harmony.PatchAll(typeof(BoomboxSyncFixPlugin));
            harmony.PatchAll(typeof(BoomboxItemStartMusicPatch));
            harmony.PatchAll(typeof(NetworkObjectManager));
        }
    }
}