using BoomboxSyncFix.Patches;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoomboxSyncFix.Networking
{
    [HarmonyPatch]
    public class NetworkObjectManager
    {
        static GameObject networkPrefab;

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
        public static void Init()
        {
            if (networkPrefab != null)
            {
                return;
            }

            var MainAssetBundle = AssetBundle.LoadFromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("BoomboxSyncFix.asset"));

            networkPrefab = (GameObject)MainAssetBundle.LoadAsset("BoomboxSyncNetworkHandler");
            networkPrefab.AddComponent<NetworkHandler>();

            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
            BoomboxSyncFixPlugin.Instance.logger.LogInfo("Created NetworkHandler prefab");
        }
    }
}
