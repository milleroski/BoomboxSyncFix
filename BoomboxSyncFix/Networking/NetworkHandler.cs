using HarmonyLib;
using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoomboxSyncFix.Networking
{
    public class NetworkHandler : NetworkBehaviour
    {

        public static event Action<String> LevelEvent;
        public static NetworkHandler Instance { get; private set; }

        private void Awake()
        {
            if ((Object)(object)Instance == (Object)null)
            {
                Instance = this;
                Object.DontDestroyOnLoad((Object)(object)((Component)this).gameObject);
                Debug.Log((object)"NetworkHandler instance created.");
            }
            else
            {
                Object.Destroy((Object)(object)((Component)this).gameObject);
                Debug.Log((object)"Extra NetworkHandler instance destroyed.");
            }
        }

        public override void OnNetworkSpawn()
        {
            LevelEvent = null;

            base.OnNetworkSpawn();
        }

        private void setvarsClientRpc()
        {
            
        }


    }
}