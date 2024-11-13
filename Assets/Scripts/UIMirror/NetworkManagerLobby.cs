using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEditor.MemoryProfiler;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [Scene][SerializeField] private string menuScene = "ConnectionMenu";

    //[Header("Maps")]
    //[SerializeField] private int numberOfRounds = 1;
    //[SerializeField] private MapSet mapSet = null;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    //[SerializeField] private GameObject playerSpawnSystem = null;
    //[SerializeField] private GameObject roundSystem = null;

    //private MapHandler mapHandler;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;

    public List<NetworkRoomPlayerLobby> RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

    public override void OnStartServer() => spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    //public override void OnStartClient()
    //{
    //    var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

    //    foreach (var prefab in spawnablePrefabs)
    //    {
    //        if (!NetworkClient.prefabs.ContainsKey(prefab.GetComponent<NetworkIdentity>().assetId)) // Проверяем по assetId
    //        {
    //            NetworkClient.RegisterPrefab(prefab);
    //        }
    //        else
    //        {
    //            Debug.LogWarning($"Prefab {prefab.name} уже зарегистрирован с assetId {prefab.GetComponent<NetworkIdentity>().assetId}");
    //        }
    //    }
    //}
    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            var networkIdentity = prefab.GetComponent<NetworkIdentity>();

            if (networkIdentity == null)
            {
                Debug.LogWarning($"Prefab {prefab.name} не имеет компонента NetworkIdentity и не может быть зарегистрирован.");
                continue;
            }

            if (!NetworkClient.prefabs.ContainsKey(networkIdentity.assetId))
            {
                NetworkClient.RegisterPrefab(prefab);
            }
            else
            {
                Debug.LogWarning($"Prefab {prefab.name} уже зарегистрирован с assetId {networkIdentity.assetId}");
            }
        }

        // Спавним объекты на сцене, как только клиент подключается
        SpawnObjectsOnScene();
    }

    private void SpawnObjectsOnScene()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach (var prefab in spawnablePrefabs)
        {
            // Создаем объект на сцене
            GameObject spawnedObject = Instantiate(prefab);

            // Спавним его в сети
            NetworkServer.Spawn(spawnedObject);
        }
    }


    public override void OnClientConnect()
    {
        base.OnClientConnect();

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != System.IO.Path.GetFileNameWithoutExtension(menuScene))
        {
            Debug.Log(SceneManager.GetActiveScene().name);
            Debug.Log($"Сравнение");
            Debug.Log(menuScene);
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();

            RoomPlayers.Remove(player);

            NotifyPlayersOfReadyState();
        }

        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        //GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers) { return false; }

        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady) { return false; }
        }

        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            //mapHandler = new MapHandler(mapSet, numberOfRounds);

            //ServerChangeScene(mapHandler.NextMap);
            ServerChangeScene("MenuGame");
        }
    }

    public override void ServerChangeScene(string newSceneName)
    {
        // From menu to game
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Scene_Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);

                NetworkServer.Destroy(conn.identity.gameObject);

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }

        base.ServerChangeScene(newSceneName);
    }

    //public override void OnServerSceneChanged(string sceneName)
    //{
    //    if (sceneName.StartsWith("Scene_Map"))
    //    {
    //        GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
    //        NetworkServer.Spawn(playerSpawnSystemInstance);

    //        GameObject roundSystemInstance = Instantiate(roundSystem);
    //        NetworkServer.Spawn(roundSystemInstance);
    //    }
    //}

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}