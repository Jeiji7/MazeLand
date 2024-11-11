//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections.Generic;
//using Unity.Services.Lobbies.Models;
//using TMPro;

//public class MainMenuUI : MonoBehaviour
//{
//    public TMP_InputField roomNameInputField;
//    public Toggle isPrivateToggle;
//    public Button createButton;
//    public GameObject roomListContent;
//    public GameObject roomListItemPrefab;
//    public TMP_InputField privateJoinCodeInputField;
//    public Button joinPrivateButton;

//    private void Start()
//    {
//        createButton.onClick.AddListener(() =>
//        {
//            LobbyManager.Instance.CreateLobby(roomNameInputField.text);
//        });

//        joinPrivateButton.onClick.AddListener(() =>
//        {
//            LobbyManager.Instance.JoinLobbyById(privateJoinCodeInputField.text);
//        });

//        FetchPublicRooms();
//    }

//    private void FetchPublicRooms()
//    {
//        LobbyManager.Instance.FetchPublicLobbies(DisplayRoomList);
//    }

//    private void DisplayRoomList(List<Lobby> lobbies)
//    {
//        foreach (Transform child in roomListContent.transform)
//        {
//            Destroy(child.gameObject);
//        }

//        foreach (var lobby in lobbies)
//        {
//            GameObject listItem = Instantiate(roomListItemPrefab, roomListContent.transform);
//            listItem.GetComponentInChildren<Text>().text = lobby.Name;

//            listItem.GetComponent<Button>().onClick.AddListener(() =>
//            {
//                LobbyManager.Instance.JoinLobbyById(lobby.Id);
//            });
//        }
//    }
//}

