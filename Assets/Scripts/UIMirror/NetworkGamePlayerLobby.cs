using UnityEngine;
using Mirror;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
    [SyncVar]
    private string displayName;

    public void SetDisplayName(string name)
    {
        displayName = name;
    }
}
