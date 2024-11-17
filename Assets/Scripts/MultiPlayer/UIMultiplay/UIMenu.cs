using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public GameObject Lobby;
    public GameObject JoinClient;
    public void OpenListLobby()
    {
        Lobby.SetActive(true);
    }

    public void OpenJoinClient()
    {
        JoinClient.SetActive(true);
    }
}
