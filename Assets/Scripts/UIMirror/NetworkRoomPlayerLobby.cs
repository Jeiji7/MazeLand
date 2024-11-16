//using UnityEngine;
//using Mirror;
//using TMPro;
//using UnityEngine.UI;

//public class NetworkRoomPlayerLobby : NetworkBehaviour
//{
//    [SyncVar(hook = nameof(HandleDisplayNameChanged))]
//    public string DisplayName = "Loading...";

//    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
//    public bool IsReady = false;

//    [SerializeField] private TMP_Text playerNameText;
//    [SerializeField] private TMP_Text playerReadyText;
//    [SerializeField] private Button readyButton;
//    [SerializeField] private Button startGameButton;

//    private bool isLeader;
//    public bool IsLeader
//    {
//        set
//        {
//            isLeader = value;
//            startGameButton.gameObject.SetActive(value);
//        }
//    }

//    private NetworkManagerLobby room;
//    private NetworkManagerLobby Room
//    {
//        get
//        {
//            if (room != null) return room;
//            return room = NetworkManager.singleton as NetworkManagerLobby;
//        }
//    }

//    public override void OnStartAuthority()
//    {
//        readyButton.onClick.AddListener(CmdReadyUp);
//        startGameButton.onClick.AddListener(CmdStartGame);
//    }

//    public void HandleReadyStatusChanged(bool oldValue, bool newValue)
//    {
//        UpdateDisplay();
//    }

//    public void HandleDisplayNameChanged(string oldValue, string newValue)
//    {
//        UpdateDisplay();
//    }

//    private void UpdateDisplay()
//    {
//        playerNameText.text = DisplayName;
//        playerReadyText.text = IsReady ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
//    }

//    [Command]
//    private void CmdReadyUp()
//    {
//        IsReady = !IsReady;
//        Room.StartGame();// Проверяет, все ли готовы и запускает игру, если готовы
//    }

//    [Command]
//    public void CmdStartGame()
//    {
//        if (Room.RoomPlayers[0].connectionToClient != connectionToClient) return;
//        Room.StartGame();
//    }
//}
