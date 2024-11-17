using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefineWinner : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out NetworkPlayerSpawner component))
        {
            GameStateManager.Instance.SetGameOver();

            Debug.Log($"asndjnasjkdnasjnklmsdfklmgklsmlkfgm");
        }
    }
}
