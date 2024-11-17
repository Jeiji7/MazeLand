using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class FontLobbyUI : MonoBehaviour
{
    [SerializeField] private Button _createLobbyBtn;
    [SerializeField] private Button _quickJoinLobbyBtn;
    [SerializeField] private Button _joinLobbyBtn;
    [SerializeField] private TMP_InputField _lobbyCodeInputField;

    private void Awake()
    {
        Init();
    }

    public void Init()
    {
        Dispose();
        InitButton();
    }

    private void OnDisable()
    {
        Dispose();
    }

    private void InitButton()
    {
        ActivateButtons();
        _createLobbyBtn.onClick.AddListener(TestCreateLobby);
        _joinLobbyBtn.onClick.AddListener(TestJoinWithCode);
        _quickJoinLobbyBtn.onClick.AddListener(TestQuickJoin);
    }

    private void Dispose()
    {
        DeactivateButtons();
        _createLobbyBtn.onClick.RemoveListener(TestCreateLobby);
        _joinLobbyBtn.onClick.RemoveListener(TestJoinWithCode);
    }

    private void ActivateButtons()
    {
        _createLobbyBtn.gameObject.SetActive(true);
        _joinLobbyBtn.gameObject.SetActive(true);
        _quickJoinLobbyBtn.gameObject.SetActive(true);
        _lobbyCodeInputField.gameObject.SetActive(true);
    }

    private void DeactivateButtons()
    {
        _createLobbyBtn.gameObject.SetActive(false);
        _joinLobbyBtn.gameObject.SetActive(false);
        _quickJoinLobbyBtn.gameObject.SetActive(false);
        _lobbyCodeInputField.gameObject.SetActive(false);
    }

    private async void TestQuickJoin()
    {
        await LobbyRelayManager.Instance.QuickJoin();
    }

    private async void TestJoinWithCode()
    {
        await LobbyRelayManager.Instance.JoinByCode(_lobbyCodeInputField.text);
    }

    private async void TestCreateLobby()
    {
        await LobbyRelayManager.Instance.CreateLobby("Lobby_" + Random.Range(1, 200));
    }
}
