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

        private static GameObject networkHandlerHost;

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
            BoomboxSyncFixPlugin.Instance.logger.LogInfo("Created AudioNetworkHandler prefab");
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Awake")]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var networkHandlerHost = Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameNetworkManager), "StartDisconnect")]
        private static void DestroyNetworkHandler()
        {
            try
            {
                if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
                {
                    BoomboxSyncFixPlugin.Instance.logger.LogInfo("Destroying network handler");
                    Object.Destroy((Object)(object)networkHandlerHost);
                    networkHandlerHost = null;
                }
            }
            catch
            {
                BoomboxSyncFixPlugin.Instance.logger.LogError("Failed to destroy network handler");
            }
        }
    }
}
