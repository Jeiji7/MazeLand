using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SimpleSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject rockPrefab; // Префаб камня
    [SerializeField] private Transform spawnPoint; // Точка спавна
    [SerializeField] private float throwForce = 10f; // Сила броска
    private float lastDirection = 1f; // Последнее направление (1 = вправо, -1 = влево)

    void Update()
    {
        if (!IsOwner) return;

        // Обновляем направление движения на основе нажатий клавиш
        if (Input.GetKey(KeyCode.A))
        {
            lastDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            lastDirection = 1f;
        }

        // Запуск броска
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnObjectServerRpc(lastDirection);
        }
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(float direction)
    {
        // Создаём объект только на сервере
        GameObject rockInstance = Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);

        // Синхронизируем объект через сеть
        NetworkObject networkObject = rockInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // Движение объекта
        MoveRockClientRpc(networkObject.NetworkObjectId, direction);

        // Удаляем объект через 2 секунды на сервере
        StartCoroutine(DestroyRockAfterTime(rockInstance.GetComponent<NetworkObject>(), 2f));
    }

    [ClientRpc]
    private void MoveRockClientRpc(ulong objectId, float direction)
    {
        // Находим объект по его ID
        foreach (var networkObject in FindObjectsOfType<NetworkObject>())
        {
            if (networkObject.NetworkObjectId == objectId)
            {
                Rigidbody2D rb = networkObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(direction * throwForce, 0); // Движение в указанную сторону
                }
                break;
            }
        }
    }

    private System.Collections.IEnumerator DestroyRockAfterTime(NetworkObject networkObject, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (networkObject != null && networkObject.IsSpawned)
        {
            networkObject.Despawn(); // Уничтожаем объект корректно
        }
    }
}

