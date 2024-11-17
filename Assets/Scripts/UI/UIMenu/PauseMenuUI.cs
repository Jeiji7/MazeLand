using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuUI : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private static void BackToMenu()
    {
        LobbyRelayManager.Instance.LeaveLobby();
        NetworkManager.Singleton.Shutdown();
        SceneLoader.Load(SceneLoader.Scene.ConnectionMenu);
        if (NetworkManager.Singleton != null)
        {
            Destroy(NetworkManager.Singleton.gameObject);
        }

        if (MultiplayerStorage.Instance != null)
        {
            Destroy(MultiplayerStorage.Instance.gameObject);
        }

        if (LobbyRelayManager.Instance != null)
        {
            Destroy(LobbyRelayManager.Instance.gameObject);
        }
    }
}
