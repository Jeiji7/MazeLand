using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

//public class NetworkPlayerSpawner : NetworkBehaviour
//{
//    public static NetworkPlayerSpawner LocalInstance { get; private set; }
//    //[SerializeField] private TMP_Text namePlayer;
//    [SerializeField] private string names;
//    [SerializeField] private string id;
//    [SerializeField] private int index;
//    private int _playerIndex;

//    [SerializeField] private List<Vector3> spawnPositionList;

//    public override void OnNetworkSpawn()
//    {
//        if (!IsOwner)
//            return;

//        LocalInstance = this;
//        //namePlayer.text = MultiplayerStorage.Instance.GetPlayerData().playerName.ToString();

//        ulong clientID = MultiplayerStorage.Instance.GetPlayerData().clientId;
//        int playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(clientID);
//        _playerIndex = playerIndex;
//        transform.position = spawnPositionList[playerIndex];
//        //names = MultiplayerStorage.Instance.GetPlayerData().playerName.ToString();
//        //id = MultiplayerStorage.Instance.GetPlayerData().clientId.ToString();
//        //index = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(clientID).ToString();
//        //names = PlayerPrefs.GetString("NamePlayer");
//        //index = PlayerPrefs.GetInt("IndexGame");
//        Debug.Log(index + " �����");
//        //_playerIndex = index;
//    }

//    public int GetPlayerIndex()
//    {
//        Debug.Log("� ��� ���������� ��� ��������: " + _playerIndex);
//        return _playerIndex;
//    }
//public class NetworkPlayerSpawner : NetworkBehaviour
//{
//    private NetworkList<PlayerData> _playerDataNetworkList = new NetworkList<PlayerData>();
//    public static NetworkPlayerSpawner LocalInstance { get; private set; }
//    //[SerializeField] private TMP_Text namePlayer;
//    [SerializeField] private string names;
//    [SerializeField] private string id;
//    [SerializeField] public int index;
//    private int _playerIndex;

//    [SerializeField] private List<Vector3> spawnPositionList;

//    public override void OnNetworkSpawn()
//    {
//        if (!IsOwner)
//            return;

//        LocalInstance = this;

//        //namePlayer.text = MultiplayerStorage.Instance.GetPlayerData().playerName.ToString();
//        ulong clientID = MultiplayerStorage.Instance.GetPlayerData().clientId;
//        int playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(clientID);
//        _playerIndex = index;
//        transform.position = spawnPositionList[playerIndex];
//    }

//    public int GetPlayerIndex()
//    {
//        Debug.Log("� ��� ���������� ��� ��������: " + _playerIndex);
//        return _playerIndex;
//    }
//}
public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static NetworkPlayerSpawner LocalInstance { get; private set; }
    [SerializeField] private string names;
    [SerializeField] private string id;
    [SerializeField] public int index;
    [SerializeField] private List<Vector3> spawnPositionList; // ������� ��� ������ �������
    [SerializeField] private int _playerIndex; // ������ �������� ������
    [SerializeField] private ulong _clientID; // ID �������

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        LocalInstance = this;

        // �������� ������ �������� ������ ����� MultiplayerStorage
        var playerData = MultiplayerStorage.Instance.GetPlayerData();

        if (playerData.clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogError($"������: ������ ������ �� ��������� � ��������� ��������! ��������� {NetworkManager.Singleton.LocalClientId}, � �������� {playerData.clientId}");
            return;
        }

        // ��������� ������ � ���������� ID
        _playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(playerData.clientId);
        _clientID = playerData.clientId;

        // ������������� ������� ������ ������
        if (_playerIndex >= 0 && _playerIndex < spawnPositionList.Count)
        {
            transform.position = spawnPositionList[_playerIndex];
            Debug.Log($"����� ������� ��������� �� ������� {_playerIndex}: {transform.position}");
        }
        else
        {
            Debug.LogError($"������: ������ ������ {_playerIndex} ������� �� ������� ������ ������� ({spawnPositionList.Count})!");
        }
    }

    public int GetPlayerIndex()
    {
        Debug.Log($"� ��� ���������� ��� ��������: {_playerIndex}");
        return _playerIndex;
    }
}
