using HarmonyLib;
using BoomboxSyncFix.Shared;

namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    [HarmonyPatch("StartMusic")]
    internal class BoomboxItemStartMusicPatch
    {

        public static void Prefix(ref StartOfRound ___playersManager, ref System.Random ___musicRandomizer)
        {
            BoomboxSyncFixPlugin.Instance.logger.LogInfo("In StartMusic()");

            if (___playersManager.randomMapSeed != SharedDataContainer.lastKnownVariableValue)
            {
                ___musicRandomizer = new System.Random(___playersManager.randomMapSeed - 10); //Reinitialize musicRandomizer to match the hosts randomMapSeed
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("Musicrandomizer has been synced!");

                SharedDataContainer.lastKnownVariableValue = ___playersManager.randomMapSeed;
            }
            BoomboxSyncFixPlugin.Instance.logger.LogInfo(___playersManager.randomMapSeed - 10);

        }

    }
}
