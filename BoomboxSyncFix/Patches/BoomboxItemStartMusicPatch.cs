using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;


namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch]
    internal class BoomboxItemStartMusicPatch
    {
        // Original variables to keep track of the seed syncing
        private static Dictionary<BoomboxItem, bool> seedSyncDictionary = new Dictionary<BoomboxItem, bool>();
        private static FieldInfo playersManagerField = AccessTools.Field(typeof(BoomboxItem), "playersManager");

        // Edge case variables for Late join mods
        private static Dictionary<BoomboxItem, int> musicPlayAmount = new Dictionary<BoomboxItem, int>();
        private static Dictionary<BoomboxItem, bool> playAmountSyncDictionary = new Dictionary<BoomboxItem, bool>();

        [HarmonyPatch(typeof(BoomboxItem), "StartMusic")]
        [HarmonyPrefix]
        public static void PatchStartMusic(BoomboxItem __instance)
        {
            StartOfRound playersManager = (StartOfRound)playersManagerField.GetValue(__instance);

            // The host needs to be keeping track of all the boomboxes being played and how often they get played, since they're always around.
            if (__instance.isPlayingMusic == false)
            {
                // If the Dictionary was not initialized yet, do so
                if (!musicPlayAmount.ContainsKey(__instance))
                {
                    musicPlayAmount[__instance] = 0;
                }

                musicPlayAmount[__instance] += 1;

                BoomboxSyncFixPlugin.Instance.logger.LogInfo("Incrementing musicPlayAmount by 1");
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("PlayAmount:");
                BoomboxSyncFixPlugin.Instance.logger.LogInfo(musicPlayAmount[__instance]);
            }

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

            // If the playAmount has not been synced yet and ONLY if this is the client.
            if ((!playAmountSyncDictionary.TryGetValue(__instance, out bool playAmountSynced) || !playAmountSynced) && !NetworkManager.Singleton.IsHost)
            {
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("Got in the playamountsync part.");

                BoomboxSyncFixPlugin.Instance.logger.LogInfo(musicPlayAmount[__instance]);

                // Only do this if the client joined late, a.k.a. when the host has played the song AT LEAST ONCE
                if (musicPlayAmount[__instance] > 1)
                {
                    BoomboxSyncFixPlugin.Instance.logger.LogInfo("Entering the for loop...");

                    // Run musicRandomizer.Next the amount of times that the host has played the boombox
                    for (int i = 0; i < musicPlayAmount[__instance]; i++)
                    {
                        BoomboxSyncFixPlugin.Instance.logger.LogInfo($"{i}");
                        __instance.musicRandomizer.Next();
                    }
                }

                // This boombox is synced by playAmount now, don't access this part of code anymore
                playAmountSyncDictionary[__instance] = true;
            }
        }

    }
}