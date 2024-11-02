using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BlindZone : MonoBehaviour
{
    public Transform player;  // ������ �� ������ ������
    public float returnSpeed = 2f;  // �������� �������� ���� � ������
    public float bufferDistance = 0.5f;  // �������� ����������, �� ������� ����� ����� �������� �� ������� ����
    private CinemachineVirtualCamera virtualCamera;
    private Transform originalFollowTarget;
    private bool playerIsInBlindZone = true;  // ����� ��������� � ����

    private Vector2 blindZoneSize;  // ������ "������ ����"

    void Start()
    {
        player = CellularAutomataDungeon.playerTransform;
        // ������� ����������� ������ �� �����
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            // ��������� �������� ���� ���������� ������
            originalFollowTarget = virtualCamera.Follow;
        }

        // �������� ������� ���������� "������ ����"
        blindZoneSize = GetComponent<BoxCollider2D>().size;
    }

    void Update()
    {
        // ���������, ��� ��������� �����
        Vector2 playerPosition = player.position;
        Vector2 blindZonePosition = transform.position;

        // ���������, ������� �� ����� �� ������� ���� � ������ ������
        if (Mathf.Abs(playerPosition.x - blindZonePosition.x) > blindZoneSize.x / 2 + bufferDistance ||
            Mathf.Abs(playerPosition.y - blindZonePosition.y) > blindZoneSize.y / 2 + bufferDistance)
        {
            // ��������� "������ ����", ������ �������� ��������� �� �������
            playerIsInBlindZone = false;
            virtualCamera.Follow = player;
        }

        // ���� ����� ����������� � ��������� �� ��������� ����
        if (!playerIsInBlindZone && player.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            // ������ ���������� "������ ����" �� ������
            transform.position = Vector2.Lerp(transform.position, player.position, Time.deltaTime * returnSpeed);

            // ���� ���� ���������� ������ � ������, ��������������� �������� ���������
            if (Vector2.Distance(transform.position, player.position) < 0.1f)
            {
                playerIsInBlindZone = true;
                virtualCamera.Follow = null;  // ������������� ������, ������ ���� ����������
            }
        }
    }

    // ����� ����� ������ � "������ ����"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInBlindZone = true;
            virtualCamera.Follow = null;  // ������������� ������
        }
    }

    // ����� ����� ������� �� "������ ����"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInBlindZone = false;
            virtualCamera.Follow = player;  // ������ �������� ��������� �� �������
        }
    }
}
