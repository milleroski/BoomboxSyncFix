using HarmonyLib;
using System.Collections.Generic;

namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    [HarmonyPatch("StartMusic")]
    internal class BoomboxItemStartMusicPatch
    {
        private static Dictionary<BoomboxItem, bool> seedSyncDictionary = new Dictionary<BoomboxItem, bool>();

        public static void Prefix(BoomboxItem __instance, ref StartOfRound ___playersManager, ref System.Random ___musicRandomizer)
        {
            if (!seedSyncDictionary.TryGetValue(__instance, out bool seedSynced) || !seedSynced)
            {
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("In StartMusic()");

                if (___playersManager.randomMapSeed > 0)
                {
                    ___musicRandomizer = new System.Random(___playersManager.randomMapSeed - 10);
                    BoomboxSyncFixPlugin.Instance.logger.LogInfo("Musicrandomizer variable has been synced!");
                    seedSyncDictionary[__instance] = true;
                }

                BoomboxSyncFixPlugin.Instance.logger.LogInfo(___playersManager.randomMapSeed - 10);
            }
        }
    }
}
