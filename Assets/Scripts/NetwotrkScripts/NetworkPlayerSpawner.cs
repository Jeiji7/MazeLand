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
//        Debug.Log(index + " Игрок");
//        //_playerIndex = index;
//    }

//    public int GetPlayerIndex()
//    {
//        Debug.Log("И наш победитель под индексом: " + _playerIndex);
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
//        Debug.Log("И наш победитель под индексом: " + _playerIndex);
//        return _playerIndex;
//    }
//}
public class NetworkPlayerSpawner : NetworkBehaviour
{
    public static NetworkPlayerSpawner LocalInstance { get; private set; }
    [SerializeField] private string names;
    [SerializeField] private string id;
    [SerializeField] public int index;
    [SerializeField] private List<Vector3> spawnPositionList; // Позиции для спавна игроков
    [SerializeField] private int _playerIndex; // Индекс текущего игрока
    [SerializeField] private ulong _clientID; // ID клиента

    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
            return;

        LocalInstance = this;

        // Получаем данные текущего игрока через MultiplayerStorage
        var playerData = MultiplayerStorage.Instance.GetPlayerData();

        if (playerData.clientId != NetworkManager.Singleton.LocalClientId)
        {
            Debug.LogError($"Ошибка: данные игрока не совпадают с локальным клиентом! Ожидалось {NetworkManager.Singleton.LocalClientId}, а получено {playerData.clientId}");
            return;
        }

        // Сохраняем индекс и клиентский ID
        _playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(playerData.clientId);
        _clientID = playerData.clientId;

        // Устанавливаем позицию спавна игрока
        if (_playerIndex >= 0 && _playerIndex < spawnPositionList.Count)
        {
            transform.position = spawnPositionList[_playerIndex];
            Debug.Log($"Игрок успешно заспавнен на позиции {_playerIndex}: {transform.position}");
        }
        else
        {
            Debug.LogError($"Ошибка: индекс игрока {_playerIndex} выходит за границы списка позиций ({spawnPositionList.Count})!");
        }
    }

    public int GetPlayerIndex()
    {
        Debug.Log($"И наш победитель под индексом: {_playerIndex}");
        return _playerIndex;
    }
}
