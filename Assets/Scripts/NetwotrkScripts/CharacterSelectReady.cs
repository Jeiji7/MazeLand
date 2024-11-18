using System.Collections.Generic;
using System.Diagnostics;
using Unity.Netcode;
using UnityEngine.Events;
using UnityEngine;
using Mirror.BouncyCastle.Tls;


public class CharacterSelectReady : NetworkBehaviour
{
    public static CharacterSelectReady Instance { get; private set; }

    internal UnityEvent OnReadyChanged = new();

    private Dictionary<ulong, bool> playerReadyDictionary;

    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
    }

    private void Start()
    {
        AddNewkey(MultiplayerStorage.Instance.GetPlayerData().clientId);
    }

    private void AddNewkey(ulong clientID)
    {
        playerReadyDictionary.Add(clientID, false);
        UnityEngine.Debug.LogError($"New key {clientID}");
    }

    public void SetPlayerReady()
    {
        if (NetworkManager.IsHost)
        {
            UnityEngine.Debug.LogError($"IS hosh");
            playerReadyDictionary[0] = true;
            LobbyRelayManager.Instance.DeleteLobby();
            switch (LobbyGameSceneUI._sceneNumber)
            {
                case 0:
                    SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
                    break;
                case 1:
                    SceneLoader.LoadNetwork(SceneLoader.Scene.GameSceneTwo);
                    break;
                case 2:
                    SceneLoader.LoadNetwork(SceneLoader.Scene.GameSceneThree);
                    break;
            }
            //CursorController.DisableCursor();
            OnReadyChanged.Invoke();
            LobbyGameSceneUI._sceneNumber = -1;
        }
        else
            SetPlayerReadyServerRpc();
    }

    [ServerRpc(RequireOwnership = false)] //метод не может вызвать хост\сервер, только клиент
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        UnityEngine.Debug.LogError($"SetReady pressed server rpc");
        SetPlayerReadyClientRpc(serverRpcParams.Receive.SenderClientId);

        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientsReady = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                // This player is NOT ready
                allClientsReady = false;
                break;
            }
        }

        if (allClientsReady)
        {
            LobbyRelayManager.Instance.DeleteLobby();
            SceneLoader.LoadNetwork(SceneLoader.Scene.GameScene);
            //CursorController.DisableCursor();
        }
    }

    [ClientRpc]
    private void SetPlayerReadyClientRpc(ulong clientId)
    {
        playerReadyDictionary[clientId] = true;

        OnReadyChanged.Invoke();
    }


    public bool IsPlayerReady(ulong clientId)
    {
        try
        {
            return playerReadyDictionary.ContainsKey(clientId) && playerReadyDictionary[clientId];
        }
        catch
        {
            UnityEngine.Debug.LogError($"Error here");
            return false;

        }
    }

}