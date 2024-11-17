using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static NetworkPlayerSpawner LocalInstance { get; private set; }

    private ulong _clientID;

    [SerializeField] private List<Vector3> spawnPositionList;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        LocalInstance = this;

        ulong clientID = MultiplayerStorage.Instance.GetPlayerData().clientId;
        _clientID = clientID;   
        int playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(clientID);
        transform.position = spawnPositionList[playerIndex];
    }

    public ulong GetPlayerClientID()
    {
        return _clientID;
    }
}
