//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.ConstrainedExecution;
//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;
//using Unity.Services.Authentication;
//using Unity.Services.Core;
//using Unity.Services.Relay;
//using Unity.Services.Relay.Models;
//using UnityEditor.Experimental.GraphView;
//using UnityEngine;

//public class TestRelay : MonoBehaviour
//{
//    //private async void Start()
//    //{
//    //    await UnityServices.InitializeAsync();

//    //    AuthenticationService.Instance.SignedIn += () =>
//    //    {
//    //        Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
//    //        FindObjectOfType<ConsoleController>().LogMessage("Signed in " + AuthenticationService.Instance.PlayerId);
//    //    };
//    //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
//    //}
//    //[Command("CreateRelay")]
//    //private async void CreateRelay()
//    //{
//    //    try
//    //    {
//    //        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
//    //        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
//    //        Debug.Log(joinCode);
//    //        FindObjectOfType<ConsoleController>().LogMessage(joinCode);
//    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
//    //        allocation.RelayServer.IpV4,
//    //        (ushort) allocation.RelayServer.Port,
//    //        allocation.AllocationIdBytes,
//    //        allocation.Key,
//    //        allocation.ConnectionData);

//    //        NetworkManager.Singleton.StartHost();
//    //    }
//    //    catch (RelayServiceException e)
//    //    {
//    //        Debug.Log(e);
//    //        FindObjectOfType<ConsoleController>().LogMessage($"{e}");
//    //    }
//    //}
//    //[Command("JoinRelay")]
//    //private async void JoinRelay(string joinCode)
//    //{
//    //    try
//    //    {
//    //        Debug.Log("Joining Relay with " + joinCode);
//    //        FindObjectOfType<ConsoleController>().LogMessage("Joining Relay with " + joinCode);
//    //        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

//    //        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
//    //            joinAllocation.RelayServer.IpV4,
//    //            (ushort)joinAllocation.RelayServer.Port,
//    //            joinAllocation.AllocationIdBytes,
//    //            joinAllocation.Key,
//    //            joinAllocation.ConnectionData,
//    //            joinAllocation.HostConnectionData);
//    //        NetworkManager.Singleton.StartClient();
//    //    }
//    //    catch (RelayServiceException e)
//    //    {
//    //        Debug.Log(e);
//    //        FindObjectOfType<ConsoleController>().LogMessage($"{e}");
//    //    }
//    //}

//    //[Command("Loger")]
//    //private void Loger()
//    //{
//    //    FindObjectOfType<ConsoleController>().LogMessage("Amir Гуседуш");
//    //}
//    public static event Action OnUserSignedIn; // Событие для уведомления о завершении входа

//    private async void Start()
//    {
//        await UnityServices.InitializeAsync();

//        AuthenticationService.Instance.SignedIn += () =>
//        {
//            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
//            FindObjectOfType<ConsoleController>().LogMessage("Signed in " + AuthenticationService.Instance.PlayerId);
//            OnUserSignedIn?.Invoke();  // Оповещаем подписчиков, что вход выполнен
//        };

//        await AuthenticationService.Instance.SignInAnonymouslyAsync();
//    }

//    // Метод для создания Relay
//    public async void CreateRelay()
//    {
//        try
//        {
//            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);
//            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

//            Debug.Log("Join Code: " + joinCode);
//            FindObjectOfType<ConsoleController>().LogMessage("Join Code: " + joinCode + " Player Name: " + PlayerData.Instance.PlayerName);

//            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
//                allocation.RelayServer.IpV4,
//                (ushort)allocation.RelayServer.Port,
//                allocation.AllocationIdBytes,
//                allocation.Key,
//                allocation.ConnectionData);

//            NetworkManager.Singleton.StartHost();
//        }
//        catch (RelayServiceException e)
//        {
//            Debug.LogError(e);
//        }
//    }

//    // Метод для присоединения к Relay
//    public async void JoinRelay(string joinCode)
//    {
//        try
//        {
//            Debug.Log("Joining Relay with code: " + joinCode);
//            FindObjectOfType<ConsoleController>().LogMessage("Joining Relay with code: " + joinCode + " Player Name: " + PlayerData.Instance.PlayerName);

//            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

//            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
//                joinAllocation.RelayServer.IpV4,
//                (ushort)joinAllocation.RelayServer.Port,
//                joinAllocation.AllocationIdBytes,
//                joinAllocation.Key,
//                joinAllocation.ConnectionData,
//                joinAllocation.HostConnectionData);

//            NetworkManager.Singleton.StartClient();
//        }
//        catch (RelayServiceException e)
//        {
//            Debug.LogError(e);
//        }
//    }
//}
