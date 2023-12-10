using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch]
    internal class BoomboxItemStartMusicPatch
    {
        // Variables to keep track of the seed syncing across multiple BoomboxItem instances
        private static Dictionary<BoomboxItem, bool> seedSyncDictionary = new Dictionary<BoomboxItem, bool>();
        private static FieldInfo playersManagerField = AccessTools.Field(typeof(BoomboxItem), "playersManager");

        [HarmonyPatch(typeof(BoomboxItem), "StartMusic")]
        [HarmonyPrefix]
        public static void StartMusicPatch(BoomboxItem __instance)
        {
            StartOfRound playersManager = (StartOfRound)playersManagerField.GetValue(__instance);

            // If the seed has not been synced yet for the musicRandomizer
            if (!seedSyncDictionary.TryGetValue(__instance, out bool seedSynced) || !seedSynced)
            {
                // If the seed has actually initialized (value bigger than 0) 
                if (playersManager != null && playersManager.randomMapSeed > 0)
                {
                    int sharedSeed = playersManager.randomMapSeed - 10;
                    __instance.musicRandomizer = new System.Random(sharedSeed);
                    BoomboxSyncFixPlugin.Instance.logger.LogInfo($"Musicrandomizer variable has been synced with seed: {sharedSeed}");
                    seedSyncDictionary[__instance] = true;
                }
            }
        }

        // This happens only in the case that a "late join" mod is uses like LateCompany or ShipLobby -- you can't use the boombox before you start the game normally
        [HarmonyPatch(typeof(StartOfRound), "OnPlayerConnectedClientRpc")]
        [HarmonyPrefix]
        public static void OnPlayerConnectedPatch(StartOfRound __instance)
        {
            BoomboxSyncFixPlugin.Instance.logger.LogInfo($"Another client joined -- forcing everyone to reintialize musicRandomizer.");
            forceReinitialize(seedSyncDictionary);

        }

        // This is for a weird edge case when the host starts playing the boombox before the game has loaded in -- this makes sure everyone gets resynced when the level has loaded in.
        [HarmonyPatch(typeof(StartOfRound), "openingDoorsSequence")]
        [HarmonyPrefix]
        public static void openingDoorsSequencePatch(StartOfRound __instance)
        {
            BoomboxSyncFixPlugin.Instance.logger.LogInfo($"Round has loaded for all -- forcing everyone to reinitialize musicRandomizer.");
            forceReinitialize(seedSyncDictionary);
        }

        private static void forceReinitialize(Dictionary<BoomboxItem, bool> seedSyncDictionary)
        {
            // Reset playAmountSyncDictionary for all BoomboxItem instances
            foreach (var boomboxItem in seedSyncDictionary.Keys.ToList())
            {
                seedSyncDictionary[boomboxItem] = false;
            }
        }
    }
}