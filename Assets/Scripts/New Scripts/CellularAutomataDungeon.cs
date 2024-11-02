using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellularAutomataDungeon : MonoBehaviour
{
    public int width = 70;
    public int height = 70;
    public float fillPercent = 0.45f;
    public int smoothSteps = 5;
    public float stepDelay = 0.5f;

    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject chestPrefab;
    public GameObject exitPrefab;
    public GameObject playerPrefab;
    public static Transform playerTransform;
    public GameObject cameraPlayerPrefab;

    private int[,] map;
    private GameObject mapHolder;
    private GameObject exitInstance;
    private bool exitSpawned = false;

    private Vector2 playerStartPos;
    private Vector2 exitPos;

    void Start()
    {
        ClearMap();
        StartCoroutine(GenerateMapWithSteps());
    }

    IEnumerator GenerateMapWithSteps()
    {
        GenerateMap();

        if (!exitSpawned)
        {
            // Спауним портал на фиксированной позиции
            SpawnExit();
            exitSpawned = true;
        }

        RenderMap();
        yield return new WaitForSeconds(stepDelay);

        for (int i = 0; i < smoothSteps; i++)
        {
            SmoothMap();
            ClearMap();
            RenderMap();

            if (exitInstance != null)
            {
                exitInstance.transform.SetParent(null); // Открепляем портал от контейнера
            }

            yield return new WaitForSeconds(stepDelay);
        }

        // Спауним сундук после завершения генерации
        SpawnChest();

        // Спауним игрока и портал на фиксированных позициях с очищением вокруг них
        SpawnPlayer();

        // Генерация туннеля от игрока к порталу
        GenerateTunnel(playerStartPos, exitPos);
    }

    void GenerateMap()
    {
        map = new int[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    map[x, y] = 1; // Границы карты — стены
                }
                else
                {
                    map[x, y] = (Random.value < fillPercent) ? 1 : 0; // Случайная генерация стен
                }
            }
        }
    }

    void SmoothMap()
    {
        int[,] newMap = new int[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int neighborWallCount = GetSurroundingWallCount(x, y);

                if (neighborWallCount > 4)
                    newMap[x, y] = 1;
                else if (neighborWallCount < 4)
                    newMap[x, y] = 0;
                else
                    newMap[x, y] = map[x, y];
            }
        }

        map = newMap;
    }

    int GetSurroundingWallCount(int gridX, int gridY)
    {
        int wallCount = 0;
        for (int neighborX = gridX - 1; neighborX <= gridX + 1; neighborX++)
        {
            for (int neighborY = gridY - 1; neighborY <= gridY + 1; neighborY++)
            {
                if (neighborX >= 0 && neighborX < width && neighborY >= 0 && neighborY < height)
                {
                    if (neighborX != gridX || neighborY != gridY)
                    {
                        wallCount += map[neighborX, neighborY];
                    }
                }
                else
                {
                    wallCount++;
                }
            }
        }
        return wallCount;
    }

    void ClearMap()
    {
        if (mapHolder != null)
        {
            Destroy(mapHolder);
        }
        mapHolder = new GameObject("Map Holder");
    }

    void RenderMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(x, y, 0);

                if (map[x, y] == 1)
                {
                    Instantiate(wallPrefab, pos, Quaternion.identity, mapHolder.transform);
                }
                else
                {
                    Instantiate(floorPrefab, pos, Quaternion.identity, mapHolder.transform);
                }
            }
        }
    }

    void SpawnChest()
    {
        Vector2 chestPos;
        do
        {
            int chestX = Random.Range(1, width - 1);
            int chestY = Random.Range(1, height - 1);
            chestPos = new Vector2(chestX, chestY);
        } while (map[(int)chestPos.x, (int)chestPos.y] != 0);

        Instantiate(chestPrefab, new Vector3(chestPos.x, chestPos.y, -2), Quaternion.identity, mapHolder.transform);
    }

    // Спауним портал на фиксированной позиции (width - 5, height - 5)
    void SpawnExit()
    {
        int exitX = width - 5;
        int exitY = height - 5;
        exitPos = new Vector2(exitX, exitY);

        exitInstance = Instantiate(exitPrefab, new Vector3(exitX, exitY, -2), Quaternion.identity);

        // Очищаем область вокруг портала (3x3)
        ClearSurroundingArea(exitX, exitY);
    }

    // Спауним игрока на позиции (5, 5)
    void SpawnPlayer()
    {
        int playerX = 5;
        int playerY = 5;

        playerStartPos = new Vector2(playerX, playerY);
        Vector3 playerPos = new Vector3(playerX, playerY, -3);
        Vector3 spawnPositionCamera = new Vector3(playerX, playerY, -10);
        GameObject playerInstance = Instantiate(playerPrefab, playerPos, Quaternion.identity);
        playerTransform = playerInstance.transform;
        // Спауним виртуальную камеру
        GameObject cameraInstance = Instantiate(cameraPlayerPrefab, spawnPositionCamera, Quaternion.identity);

        // Устанавливаем следование камеры за игроком
        CinemachineVirtualCamera virtualCam = cameraInstance.GetComponent<CinemachineVirtualCamera>();

        if (virtualCam != null)
        {
            virtualCam.Follow = playerInstance.transform; // Камера будет следить за игроком
            virtualCam.LookAt = playerInstance.transform; // Камера будет следить за игроком
        }
        else
        {
            Debug.LogError("VirtualCamera компонент не найден на объекте CameraPlayer!");
        }

        // Очищаем область вокруг игрока (3x3)
        ClearSurroundingArea(playerX, playerY);
    }

    // Удаляем стены вокруг заданной позиции
    void ClearSurroundingArea(int centerX, int centerY)
    {
        for (int x = centerX - 1; x <= centerX + 1; x++)
        {
            for (int y = centerY - 1; y <= centerY + 1; y++)
            {
                if (x >= 0 && x < width && y >= 0 && y < height)
                {
                    map[x, y] = 0; // Очищаем клетки (ставим пол)
                }
            }
        }
    }

    // Генерация туннеля от точки игрока к порталу
    void GenerateTunnel(Vector2 start, Vector2 end)
    {
        Vector2 currentPos = start;

        while (currentPos != end)
        {
            int deltaX = (int)(end.x - currentPos.x);
            int deltaY = (int)(end.y - currentPos.y);

            // Выбираем направление движения
            if (Random.value < 0.5f)
            {
                // Двигаемся по оси X
                currentPos.x += Mathf.Clamp(deltaX, -1, 1);
            }
            else
            {
                // Двигаемся по оси Y
                currentPos.y += Mathf.Clamp(deltaY, -1, 1);
            }

            int currentX = (int)currentPos.x;
            int currentY = (int)currentPos.y;

            // Удаляем стену и создаём пол на текущей позиции
            if (map[currentX, currentY] == 1) // Если это стена
            {
                // Найти и уничтожить объект стены
                foreach (Transform child in mapHolder.transform)
                {
                    if (child.position.x == currentX && child.position.y == currentY)
                    {
                        Destroy(child.gameObject); // Удаляем объект стены
                        break;
                    }
                }

                // Обновляем массив
                map[currentX, currentY] = 0; // Удаляем стену из массива
                Vector3 pos = new Vector3(currentX, currentY, 0);
                Instantiate(floorPrefab, pos, Quaternion.identity, mapHolder.transform); // Создаём пол
            }
        }
    }

}
