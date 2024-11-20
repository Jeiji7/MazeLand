using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;
using UnityEngine.SceneManagement;
using System.Linq;

public class LobbyRelayManager : MonoBehaviour
{
    internal UnityEvent OnCreateLobbyStarted = new();
    internal UnityEvent OnCreateLobbyFailed = new();
    internal UnityEvent OnJoinStarted = new();
    internal UnityEvent OnQuickJoinFailed = new();
    internal UnityEvent OnJoinFailed = new();
    internal UnityEvent OnSignIn = new();

    public static LobbyRelayManager Instance { get; private set; }

    private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";
    public const string KEY_PLAYER_NAME = "PlayerName";

    private float _heartbeatTimer;
    private float _lobbyPollTimer;
    private Lobby _joinedLobby;
    private string _playerName;


    public void Init()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeAuthentication(string playerName)
    {
        if (string.IsNullOrEmpty(playerName) || playerName.Any(char.IsWhiteSpace) || playerName.Any(ch => ",.;:".Contains(ch)))
            return;
        InitializeUnityAuthentication(playerName);
        SceneLoader.Load(SceneLoader.Scene.MenuGame);
    }
    public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
    public class OnLobbyListChangedEventArgs : EventArgs
    {
        public List<Lobby> lobbyList;
    }

    private void Update()
    {
        HandleHeartbeat();
        HandlePeriodicListLobbies();
    }

    public string GetPlayerName()
    {
        return PlayerPrefs.GetString("PlayerName");
    }

    public async Task CreateLobby(string lobbyName)
    {
        OnCreateLobbyStarted?.Invoke();
        try
        {
            if (!AuthenticationService.Instance.IsSignedIn || !AuthenticationService.Instance.IsAuthorized)
            {
                Debug.LogError("NO AUTH");
                return;
            }
            _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, MultiplayerStorage.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
            {
                IsPrivate = false,
            });

            Allocation allocation = await AllocateRelay();

            string relayJoinCode = await GetRelayJoinCode(allocation);

            await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject> {
                     { KEY_RELAY_JOIN_CODE , new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode) }
                 }
            });

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

            MultiplayerStorage.Instance.StartHost();
            SceneLoader.LoadNetwork(SceneLoader.Scene.LobbyGame);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnCreateLobbyFailed?.Invoke();
        }
    }

    public async Task QuickJoin()
    {
        OnJoinStarted?.Invoke();
        try
        {
            _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            Debug.Log($"{gameObject.name}: quick joined");

            MultiplayerStorage.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnQuickJoinFailed?.Invoke();
        }
    }

    public async Task JoinByCode(string lobbyCode)
    {
        OnJoinStarted?.Invoke();
        try
        {
            _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

            string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

            JoinAllocation joinAllocation = await JoinRelay(relayJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().
                SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
            Debug.Log($"{gameObject.name}: joined by code");

            MultiplayerStorage.Instance.StartClient();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            OnJoinFailed.Invoke();
        }
    }

    public async void DeleteLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

                _joinedLobby = null;
                Debug.Log($"{gameObject.name}: delete lobby");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }

    }

    public async void LeaveLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);
                _joinedLobby = null;
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async void KickPlayer(string playerId)
    {
        if (IsLobbyHost())
        {
            try
            {
                Debug.Log($"!!!!!!!!!!!!!! Kicked {playerId} {Instance.GetJoinedLobby().HostId}");
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);

                NetworkManager.Singleton.Shutdown();
                _joinedLobby = null;
                Debug.Log("!!!!!!!!!!!!!!!!!!!!! Shutdown !!!!!!!!!!!!!!!!!!!!!");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }
    public Lobby GetJoinedLobby()
    {
        return _joinedLobby;
    }

    public bool IsLobbyHost()
    {
        return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    private async void InitializeUnityAuthentication(string playerName)
    {
        _playerName = playerName; // ��������� ���
        PlayerPrefs.SetString("PlayerName", _playerName); // ��������� ���������� ���
        MultiplayerStorage.Instance.SetPlayerName(_playerName);

        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            InitializationOptions initializationOptions = new();
            initializationOptions.SetProfile(_playerName);

            await UnityServices.InitializeAsync(initializationOptions);

            AuthenticationService.Instance.SignedIn += () =>
            {
                Debug.Log($"Signed in! Player name: {_playerName}");
            };

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            OnSignIn.Invoke();
        }
        else
            OnSignIn.Invoke();
    }


    private void HandlePeriodicListLobbies()
    {
        if (_joinedLobby == null &&
            UnityServices.State == ServicesInitializationState.Initialized &&
            AuthenticationService.Instance.IsSignedIn &&
            SceneManager.GetActiveScene().name == SceneLoader.Scene.LobbyGame.ToString())
        {

            _lobbyPollTimer -= Time.deltaTime;
            if (_lobbyPollTimer <= 0f)
            {
                float listLobbiesTimerMax = 3f;
                _lobbyPollTimer = listLobbiesTimerMax;
                Debug.Log($"{gameObject.name}: HandlePeriodicListLobbies");
                ListLobbies();
            }
        }
    }

    private void HandleHeartbeat()
    {
        if (IsLobbyHost())
        {
            _heartbeatTimer -= Time.deltaTime;
            if (_heartbeatTimer <= 0f)
            {
                float heartbeatTimerMax = 15f;
                _heartbeatTimer = heartbeatTimerMax;

                LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
            }
        }
    }

    private async void ListLobbies()
    {
        try
        {
            QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
            {
                Filters = new List<QueryFilter> {
                  new (QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
             }
            };
            QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

            OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs
            {
                lobbyList = queryResponse.Results
            });
            Debug.Log($"{gameObject.name}: Update listLobbies");
            foreach (var item in queryResponse.Results)
            {
                Debug.Log($"{gameObject.name}: queryResponse {item.Name}");
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async Task<Allocation> AllocateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MultiplayerStorage.MAX_PLAYER_AMOUNT - 1);

            return allocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);

            return default;
        }
    }

    private async Task<string> GetRelayJoinCode(Allocation allocation)
    {
        try
        {
            string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            return relayJoinCode;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }

    private async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log($"{gameObject.name}: joined relay");
            return joinAllocation;
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
    }
}