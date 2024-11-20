using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class DefineWinner : NetworkBehaviour
{
    public int index;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NetworkPlayerSpawner component))
        {
            GameOverWinUI.Instance.SetPlayerIndex(component.GetPlayerIndex());

        }
    }
}



