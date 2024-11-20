using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class SimpleSpawner : NetworkBehaviour
{
    [SerializeField] private GameObject rockPrefab; // ������ �����
    [SerializeField] private Transform spawnPoint; // ����� ������
    [SerializeField] private float throwForce = 10f; // ���� ������
    private float lastDirection = 1f; // ��������� ����������� (1 = ������, -1 = �����)

    void Update()
    {
        if (!IsOwner) return;

        // ��������� ����������� �������� �� ������ ������� ������
        if (Input.GetKey(KeyCode.A))
        {
            lastDirection = -1f;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            lastDirection = 1f;
        }

        // ������ ������
        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnObjectServerRpc(lastDirection);
        }
    }

    [ServerRpc]
    private void SpawnObjectServerRpc(float direction)
    {
        // ������ ������ ������ �� �������
        GameObject rockInstance = Instantiate(rockPrefab, spawnPoint.position, Quaternion.identity);

        // �������������� ������ ����� ����
        NetworkObject networkObject = rockInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // �������� �������
        MoveRockClientRpc(networkObject.NetworkObjectId, direction);

        // ������� ������ ����� 2 ������� �� �������
        StartCoroutine(DestroyRockAfterTime(rockInstance.GetComponent<NetworkObject>(), 2f));
    }

    [ClientRpc]
    private void MoveRockClientRpc(ulong objectId, float direction)
    {
        // ������� ������ �� ��� ID
        foreach (var networkObject in FindObjectsOfType<NetworkObject>())
        {
            if (networkObject.NetworkObjectId == objectId)
            {
                Rigidbody2D rb = networkObject.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = new Vector2(direction * throwForce, 0); // �������� � ��������� �������
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
            networkObject.Despawn(); // ���������� ������ ���������
        }
    }
}

