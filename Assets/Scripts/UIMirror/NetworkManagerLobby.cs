using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("Host started!");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("Client started!");
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        base.OnServerConnect(conn);
        Debug.Log("Client connected!");
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);
        Debug.Log("Client disconnected!");
    }


}
