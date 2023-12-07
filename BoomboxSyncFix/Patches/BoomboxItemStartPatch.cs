using BepInEx.Logging;
using BoomboxSyncFix.Shared;
using HarmonyLib;

namespace BoomboxSyncFix.Patches
{
    [HarmonyPatch(typeof(BoomboxItem))]
    [HarmonyPatch("Start")]
    internal class BoomboxItemStartPatch
    {
        //private static bool stopUpdating = false;

        // Postfix method
        public static void Postfix(ref StartOfRound ___playersManager, ref System.Random ___musicRandomizer)
        {
            BoomboxSyncFixPlugin.Instance.logger.LogInfo("In Start()");

            if (SharedDataContainer.lastKnownVariableValue == -1)
            {
                // Set the initial value on the first frame
                SharedDataContainer.lastKnownVariableValue = ___playersManager.randomMapSeed;
            }

            // If the value changes at all, that means that it has received the seed from the server.
            else if (___playersManager.randomMapSeed != SharedDataContainer.lastKnownVariableValue)
            {
                ___musicRandomizer = new System.Random(___playersManager.randomMapSeed - 10); //Reinitialize musicRandomizer to match the hosts randomMapSeed
                BoomboxSyncFixPlugin.Instance.logger.LogInfo("Musicrandomizer has been synced!");

                SharedDataContainer.lastKnownVariableValue = ___playersManager.randomMapSeed;
            }
        }

        //private static bool stopUpdating = false; // Variable to make sure that the musicRandomizer gets updated just once when it's ready 

        //static void Postfix(ref StartOfRound ___playersManager, ref System.Random ___musicRandomizer)
        //{   
        //    // If musicRandomizer has been synced already, don't do it again
        //    if (stopUpdating)
        //    {
        //        return;
        //    }

        //    BoomboxSyncFixPlugin.Instance.logger.LogInfo("In Update()");

        //    // The value of randomMapSeed is 0 only in the case that it has not been synced yet with the server
        //    if(___playersManager.randomMapSeed > 0)
        //    {
        //        ___musicRandomizer = new System.Random(___playersManager.randomMapSeed - 10); //Reinitialize musicRandomizer to match the hosts randomMapSeed
        //        BoomboxSyncFixPlugin.Instance.logger.LogInfo("Musicrandomizer has been synced!");
        //        stopUpdating = true;
        //    } 
        //}
    }
}
