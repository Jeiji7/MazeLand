using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class GameStateManager : NetworkBehaviour
{
    public static GameStateManager Instance { get; private set; }

    internal UnityEvent OnStateChanged = new();
    internal UnityEvent OnLocalGamePaused = new();
    internal UnityEvent OnLocalGameUnpaused = new();
    internal UnityEvent OnMultiplayerGamePaused = new();
    internal UnityEvent OnMultiplayerGameUnpaused = new();
    internal UnityEvent OnLocalPlayerReadyChanged = new();
    internal UnityEvent OnStartGame = new();
    internal UnityEvent OnCloseHUD = new();
    internal UnityEvent OnOpenHUD = new();


    public enum State
    {
        WaitingToStart,
      
        FirstWin,
        SecondWin,
        ThirdWin,
        FourthWin
      //4 статуса что кадждый из 4 игроков выйграл
      // например firstplayerWin
    }

    [SerializeField] private Transform _player1;
    [SerializeField] private Transform _player2;
    [SerializeField] private Transform _player3;
    [SerializeField] private Transform _player4;

    private NetworkList<bool> _playerStatusList = new();
    private NetworkVariable<State> state = new(State.WaitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new(10f);
    private NetworkVariable<float> gamePlayingTimer = new(0f);
    private float gamePlayingTimerMax = 60f;
    private bool isLocalGamePaused = false;
    private NetworkVariable<bool> isGamePaused = new(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private bool autoTestGamePausedState;

    //private NetworkVariable<int> winnerIndex = new NetworkVariable<int>(-1);

    //// Когда кто-то выигрывает, вызываем этот метод
    ///// <summary>
    /////Логика победы вроде как
    ///// </summary>
    //[ServerRpc(RequireOwnership = false)]
    //public void ReportWinnerServerRpc(int playerIndex)
    //{
    //    winnerIndex.Value = playerIndex; // Синхронизируем на сервере
    //    // Можно выполнить дополнительные действия для игры (например, вызвать окончания игры).
    //}

    //public int GetWinnerIndex()
    //{
    //    return winnerIndex.Value;
    //}

    private void Awake()
    {
        Instance = this;
        Time.timeScale = 1.0f;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    public void InvokeStartGameEvent() =>
        OnStartGame.Invoke();


    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public State GetGameState()
    {
        return state.Value;
    }

    

    public void DifineWinner(int playerIndex)
    {
        Time.timeScale = 0.0f;
        Debug.Log(playerIndex + "игрок");
        switch (playerIndex)
        {
            case 0:
                state.Value = State.FirstWin;
                break;
            case 1:
                state.Value = State.SecondWin;
                break;
            case 2:
                state.Value = State.ThirdWin;
                break;
            case 3:
                state.Value = State.FourthWin;
                break;

        }
    }

    public bool IsLocalPlayerReady()
    {
        return isLocalPlayerReady;
    }

    public float GetGamePlayingTimerNormalized()
    {
        return 1 - (gamePlayingTimer.Value / gamePlayingTimerMax);
    }

    public void TogglePauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();

            OnLocalGamePaused.Invoke();
        }
        else
        {
            UnpauseGameServerRpc();

            OnLocalGameUnpaused.Invoke();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReportPlayerLostServerRpc(ulong clientId)
    {
        int playerIndex = MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(clientId);
        Debug.Log($"ReportPlayerLostServerRpc invoke player with clientID {clientId}, his index {playerIndex}");
        if (playerIndex != -1) _playerStatusList[playerIndex] = true;
        int index = 0;
        foreach (var item in _playerStatusList)
        {
            Debug.Log($"layerStatusList index {index} is dead? - {item}");
            index++;
        }

        CheckWinCondition();
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            int idPlayer =
    MultiplayerStorage.Instance.GetPlayerDataIndexFromClientId(MultiplayerStorage.Instance.GetPlayerDataFromClientId(clientId).clientId);

            Transform playerTransform;
            switch (idPlayer)
            {
                case 0:
                    playerTransform = Instantiate(_player1);
                    playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                    break;
                case 1:
                    playerTransform = Instantiate(_player2);
                    playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                    break;
                case 2:
                    playerTransform = Instantiate(_player3);
                    playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                    break;
                case 3:
                    playerTransform = Instantiate(_player4);
                    playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
                    break;
            }

            Debug.Log($"SceneManager_OnLoadEventCompleted added new player on status list with clientID {clientId}");
            _playerStatusList.Add(false);
        }
    }

    private void CheckWinCondition()
    {
        //пройтись по списку _playerStatusList, где true, тот и выйграл. 
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        autoTestGamePausedState = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;

            OnMultiplayerGamePaused.Invoke();
        }
        else
        {
            Time.timeScale = 1f;

            OnMultiplayerGameUnpaused.Invoke();
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke();
        Debug.LogError($"State changed from {previousValue} to {newValue}");
    }


    private void LateUpdate()
    {
        if (autoTestGamePausedState)
        {
            autoTestGamePausedState = false;
            TestGamePausedState();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;

        TestGamePausedState();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;

        TestGamePausedState();
    }

    private void TestGamePausedState()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                // This player is paused
                isGamePaused.Value = true;
                return;
            }
        }

        // All players are unpaused
        isGamePaused.Value = false;
    }
}
