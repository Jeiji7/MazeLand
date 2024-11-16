using UnityEngine;
using UnityEngine.Tilemaps;

public class ImprovedOneWayPlatform : MonoBehaviour
{
    public TilemapCollider2D platformCollider; // Коллайдер платформы
    public Transform player;            // Ссылка на игрока
    public Rigidbody2D playerRb;        // Rigidbody игрока
    public float verticalThreshold = 0.1f; // Порог для проверки вертикальной скорости

    private CapsuleCollider2D playerCollider;

    private void Start()
    {
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        if (playerRb == null)
        {
            Debug.LogError("Rigidbody2D игрока не установлен!");
        }
    }

    private void Update()
    {
        if (playerCollider != null && platformCollider != null)
        {
            float playerY = player.position.y;
            float platformY = platformCollider.bounds.max.y; // Верх платформы

            // Проверяем, если игрок под платформой и движется вверх
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
