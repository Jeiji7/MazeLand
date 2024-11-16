using UnityEngine;
using UnityEngine.Tilemaps;

public class ImprovedOneWayPlatform : MonoBehaviour
{
    public TilemapCollider2D platformCollider; // ��������� ���������
    public Transform player;            // ������ �� ������
    public Rigidbody2D playerRb;        // Rigidbody ������
    public float verticalThreshold = 0.1f; // ����� ��� �������� ������������ ��������

    private CapsuleCollider2D playerCollider;

    private void Start()
    {
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        if (playerRb == null)
        {
            Debug.LogError("Rigidbody2D ������ �� ����������!");
        }
    }

    private void Update()
    {
        if (playerCollider != null && platformCollider != null)
        {
            float playerY = player.position.y;
            float platformY = platformCollider.bounds.max.y; // ���� ���������

            // ���������, ���� ����� ��� ���������� � �������� �����
            if (playerY < platformY && playerRb.velocity.y > verticalThreshold)
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, true);
            }
            else
            {
                Physics2D.IgnoreCollision(playerCollider, platformCollider, false);
            }
        }
    }
}
