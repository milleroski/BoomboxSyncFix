using System;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BoomboxSyncFix.Networking
{
    public class NetworkHandler : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            LevelEvent = null;

            base.OnNetworkSpawn();
        }

        [ClientRpc]
        public void EventClientRpc(string eventName)
        {
            LevelEvent?.Invoke(eventName); // If the event has subscribers (does not equal null), invoke the event
        }

        public static event Action<String> LevelEvent;
    }
}