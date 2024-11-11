using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby()
    {
        if (networkManager == null)
        {
            Debug.LogError("NetworkManager is not set in MainMenu.");
            return;
        }

        networkManager.StartHost();
        landingPagePanel.SetActive(false);
    }

}
