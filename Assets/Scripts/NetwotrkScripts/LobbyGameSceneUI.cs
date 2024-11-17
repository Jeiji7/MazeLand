using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyGameSceneUI : NetworkBehaviour
{
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private Button _readyButton;
    [SerializeField] private GameObject _choiceScene;
    [SerializeField] private Button _copyCodeButton;
    [SerializeField] private TextMeshProUGUI _lobbyCodeText;

    private void Awake()
    {
        _choiceScene.SetActive(false);
        Init();
    }

    private void Init()
    {
        _mainMenuButton.onClick.AddListener(BackToMenu);
        if (NetworkManager.IsHost)
        {
            _choiceScene.SetActive(true);
            _readyButton.onClick.AddListener(SetReady);   
        }
        else
        {
            _readyButton.gameObject.SetActive(false);   
        }
        _copyCodeButton.onClick.AddListener(CopyLobbyCode);
    }

    private void CopyLobbyCode()
    {
        GUIUtility.systemCopyBuffer = _lobbyCodeText.text;
    }

    private void SetReady()
    {
        CharacterSelectReady.Instance.SetPlayerReady();
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

    private void OnEnable()
    {
        Lobby lobby = LobbyRelayManager.Instance.GetJoinedLobby();

        _lobbyCodeText.text = lobby.LobbyCode;
    }
}
