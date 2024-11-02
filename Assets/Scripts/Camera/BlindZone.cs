using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BlindZone : MonoBehaviour
{
    public Transform player;  // Ссылка на объект игрока
    public float returnSpeed = 2f;  // Скорость возврата зоны к игроку
    public float bufferDistance = 0.5f;  // Буферное расстояние, на которое игрок может выходить за границы зоны
    private CinemachineVirtualCamera virtualCamera;
    private Transform originalFollowTarget;
    private bool playerIsInBlindZone = true;  // Игрок находится в зоне

    private Vector2 blindZoneSize;  // Размер "слепой зоны"

    void Start()
    {
        player = CellularAutomataDungeon.playerTransform;
        // Находим виртуальную камеру на сцене
        virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
        if (virtualCamera != null)
        {
            // Сохраняем исходную цель следования камеры
            originalFollowTarget = virtualCamera.Follow;
        }

        // Получаем размеры коллайдера "слепой зоны"
        blindZoneSize = GetComponent<BoxCollider2D>().size;
    }

    void Update()
    {
        // Проверяем, где находится игрок
        Vector2 playerPosition = player.position;
        Vector2 blindZonePosition = transform.position;

        // Проверяем, выходит ли игрок за границы зоны с учетом буфера
        if (Mathf.Abs(playerPosition.x - blindZonePosition.x) > blindZoneSize.x / 2 + bufferDistance ||
            Mathf.Abs(playerPosition.y - blindZonePosition.y) > blindZoneSize.y / 2 + bufferDistance)
        {
            // Отключаем "слепую зону", камера начинает следовать за игроком
            playerIsInBlindZone = false;
            virtualCamera.Follow = player;
        }

        // Если игрок остановился и находится за пределами зоны
        if (!playerIsInBlindZone && player.GetComponent<Rigidbody2D>().velocity == Vector2.zero)
        {
            // Плавно возвращаем "слепую зону" на игрока
            transform.position = Vector2.Lerp(transform.position, player.position, Time.deltaTime * returnSpeed);

            // Если зона достаточно близко к игроку, восстанавливаем исходное состояние
            if (Vector2.Distance(transform.position, player.position) < 0.1f)
            {
                playerIsInBlindZone = true;
                virtualCamera.Follow = null;  // Останавливаем камеру, убирая цель следования
            }
        }
    }

    // Когда игрок входит в "слепую зону"
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInBlindZone = true;
            virtualCamera.Follow = null;  // Останавливаем камеру
        }
    }

    // Когда игрок выходит из "слепой зоны"
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerIsInBlindZone = false;
            virtualCamera.Follow = player;  // Камера начинает следовать за игроком
        }
    }
}
