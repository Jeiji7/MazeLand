using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkManager;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    //private void OnEnable()
    //{
    //    hostButton.onClick.AddListener(HostLobby);
    //    joinButton.onClick.AddListener(JoinLobby);
    //}

    //private void OnDisable()
    //{
    //    hostButton.onClick.RemoveListener(HostLobby);
    //    joinButton.onClick.RemoveListener(JoinLobby);
    //}

    public void HostLobby()
    {
        networkManager.StartHost();
    }

    public void JoinLobby()
    {
        networkManager.StartClient();
    }
}
