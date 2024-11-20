using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private int playerIndex;
    [SerializeField] private GameObject readyGameObject;
    //[SerializeField] private Button kickButton;
    [SerializeField] private TextMeshProUGUI playerNameText;


    private void Awake()
    {
        //kickButton.onClick.AddListener(KickPlayer);
    }


    private void Start()
    {
        if (MultiplayerStorage.Instance != null)
        {
            MultiplayerStorage.Instance.OnPlayerDataNetworkListChanged.AddListener(MultiplayerStorage_OnPlayerDataNetworkListChanged);
        }
        if (CharacterSelectReady.Instance != null)
        {
            CharacterSelectReady.Instance.OnReadyChanged.AddListener(CharacterSelectReady_OnReadyChanged);
        }
        UpdatePlayer();
    }


    //private void CheckKickButton(PlayerData playerData)
    //{
    //    kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);
    //}

    private void CharacterSelectReady_OnReadyChanged()
    {
        UpdatePlayer();
    }

    //private void KickPlayer()
    //{
    //    PlayerData playerData = MultiplayerStorage.Instance.GetPlayerData();
    //    Debug.Log($"pressed kick {playerData.playerId} {LobbyRelayManager.Instance.GetJoinedLobby().HostId}");
    //    LobbyRelayManager.Instance.KickPlayer(playerData.playerId.ToString());
    //    MultiplayerStorage.Instance.KickPlayerServerRpc(playerData.clientId);
    //}
    // ����� ��� ���������� �������� �������� (int)
    public void OverwriteIntData(string key, int value)
    {
        // �������������� ������������� ��������
        PlayerPrefs.SetInt(key, value);
        // ��������� ���������
        PlayerPrefs.Save();
    }
    public void OverwriteDataName(string key, string value)
    {
        // �������������� ������ � ����� ���������
        PlayerPrefs.SetString(key, value);
        // ���������� ��������� ���������, ���� ��� ���������
        PlayerPrefs.Save();
    }
    private void MultiplayerStorage_OnPlayerDataNetworkListChanged()
    {
        UpdatePlayer();
    }

    private void UpdatePlayer()
    {
        if (MultiplayerStorage.Instance.IsPlayerIndexConnected(playerIndex))
        {
            Show();
            OverwriteIntData("IndexGame", playerIndex);
            PlayerData playerData = MultiplayerStorage.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
            OverwriteDataName("NamePlayer", playerData.playerName.ToString());
            playerNameText.text = playerData.playerName.ToString();
            Debug.Log($"� ����� ������: {PlayerPrefs.GetString("NamePlayer")} ������  {PlayerPrefs.GetInt("IndexGame")}");
            //CheckKickButton(playerData);
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}