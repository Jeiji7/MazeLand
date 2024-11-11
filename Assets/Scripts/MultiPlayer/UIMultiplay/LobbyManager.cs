//using System.Collections.Generic;
//using UnityEngine;
//using Unity.Services.Lobbies;
//using Unity.Services.Lobbies.Models;
//using Unity.Services.Core;
//using Unity.Services.Authentication;
//using System;
//using System.Threading.Tasks;

//public class LobbyManager : MonoBehaviour
//{
//    public static LobbyManager Instance { get; private set; }

//    private Lobby currentLobby;

//    public event Action<List<Lobby>> OnPublicLobbiesFetched;
//    public event Action<Lobby> OnLobbyCreated;
//    public event Action<Lobby> OnJoinedLobby;
//    public event Action<string> OnLobbyError;

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void OnEnable()
//    {
//        TestRelay.OnUserSignedIn += InitializeLobby;
//    }

//    private void OnDisable()
//    {
//        TestRelay.OnUserSignedIn -= InitializeLobby;
//    }

//    private async void InitializeLobby()
//    {
//        try
//        {
//            Debug.Log("Initializing Lobby...");
//            // Вызываем FetchPublicLobbies с await
//            await FetchPublicLobbies();  // Передаем `null` или нужный callback при необходимости
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Failed to initialize lobby: {e.Message}");
//        }
//    }


//    public async void CreateLobby(string lobbyName)
//    {
//        try
//        {
//            var options = new CreateLobbyOptions
//            {
//                Data = new Dictionary<string, DataObject>
//                {
//                    { "Creator", new DataObject(DataObject.VisibilityOptions.Member, PlayerData.Instance.PlayerName) }
//                }
//            };

//            currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, 4, options);
//            Debug.Log($"Lobby '{lobbyName}' created with ID: {currentLobby.Id}");
//            OnLobbyCreated?.Invoke(currentLobby);
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to create lobby: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }

//    public async Task FetchPublicLobbies(Action<List<Lobby>> callback = null)
//    {
//        try
//        {
//            var options = new QueryLobbiesOptions
//            {
//                Filters = new List<QueryFilter>
//            {
//                new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "1", QueryFilter.OpOptions.GT)
//            }
//            };

//            var response = await LobbyService.Instance.QueryLobbiesAsync(options);
//            Debug.Log($"Fetched {response.Results.Count} lobbies");

//            // Вызываем callback, если он задан, передавая список лобби
//            callback?.Invoke(response.Results);
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to fetch lobbies: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }

//    public async void JoinLobbyById(string lobbyId)
//    {
//        try
//        {
//            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
//            Debug.Log($"Joined lobby with ID: {lobbyId}");
//            OnJoinedLobby?.Invoke(currentLobby);
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to join lobby by ID: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }

//    public async void JoinLobbyByCode(string joinCode)
//    {
//        try
//        {
//            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode);
//            Debug.Log($"Joined lobby with code: {joinCode}");
//            OnJoinedLobby?.Invoke(currentLobby);
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to join lobby by code: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }

//    public async void LeaveLobby()
//    {
//        if (currentLobby == null) return;

//        try
//        {
//            await LobbyService.Instance.RemovePlayerAsync(currentLobby.Id, AuthenticationService.Instance.PlayerId);
//            Debug.Log($"Left lobby with ID: {currentLobby.Id}");
//            currentLobby = null;
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to leave lobby: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }

//    private void Update()
//    {
//        // Periodically refresh lobby data, if necessary
//        if (currentLobby != null)
//        {
//            RefreshLobbyData();
//        }
//    }

//    private async void RefreshLobbyData()
//    {
//        if (currentLobby == null) return;

//        try
//        {
//            currentLobby = await LobbyService.Instance.GetLobbyAsync(currentLobby.Id);
//            Debug.Log($"Lobby '{currentLobby.Name}' updated.");
//        }
//        catch (LobbyServiceException e)
//        {
//            Debug.LogError($"Failed to refresh lobby data: {e.Message}");
//            OnLobbyError?.Invoke(e.Message);
//        }
//    }
//}