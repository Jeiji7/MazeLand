using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Bootstrap : MonoBehaviour
{
    //[SerializeField] private LobbyMessageUI _lobbyMessageUI;
    [SerializeField] private Button _connectBtn;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private LobbyRelayManager _lobbyRelayManager;
    [SerializeField] private MultiplayerStorage _multiplayerStorage;


    private void Awake()
    {
        _multiplayerStorage.Init();
        _lobbyRelayManager.Init();
        _connectBtn.onClick.AddListener(()=> { LobbyRelayManager.Instance.InitializeAuthentication(_inputField.text); });
        //_fontLobbyUI.Init();
        //_lobbyMessageUI.Init();
    }
}
