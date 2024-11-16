//using System.Collections;
//using System.Collections.Generic;
//using Mirror;
//using UnityEngine;

//public class DebugNetwork : MonoBehaviour
//{
//    private void Start()
//    {
//        NetworkManager.singleton.OnClientConnectCallback += OnClientConnect;
//        NetworkManager.singleton.OnClientDisconnectCallback += OnClientDisconnect;
//    }

//    private void OnClientConnect()
//    {
//        Debug.Log("Client successfully connected to the server.");
//    }

//    private void OnClientDisconnect()
//    {
//        Debug.LogError("Client disconnected from the server.");
//    }
//}
