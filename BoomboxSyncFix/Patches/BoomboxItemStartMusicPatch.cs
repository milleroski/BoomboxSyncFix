using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    [HarmonyPatch("Update")]
    internal class BoomboxItemStartMusicPatch
    {
        private static Dictionary<BoomboxItem, bool> seedSyncDictionary = new Dictionary<BoomboxItem, bool>();
        private static FieldInfo playersManagerField = AccessTools.Field(typeof(BoomboxItem), "playersManager");

        public static void Prefix(BoomboxItem __instance)
        {
            if (!seedSyncDictionary.TryGetValue(__instance, out bool seedSynced) || !seedSynced)
            {
                StartOfRound playersManager = (StartOfRound)playersManagerField.GetValue(__instance);
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("In StartMusic()");

                if (playersManager.randomMapSeed > 0)
                {
                    __instance.musicRandomizer = new System.Random(playersManager.randomMapSeed - 10);
                    BoomboxSyncFixPlugin.Instance.logger.LogInfo("Musicrandomizer variable has been synced!");
                    seedSyncDictionary[__instance] = true;
                }

                BoomboxSyncFixPlugin.Instance.logger.LogInfo(playersManager.randomMapSeed);
            }
        }
    }
}