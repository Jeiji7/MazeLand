using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuUI : NetworkBehaviour
{
    [SerializeField]private Button _mainMenuButton;
    private void Awake()
    {
        _mainMenuButton.onClick.AddListener(BackToMenu);
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
