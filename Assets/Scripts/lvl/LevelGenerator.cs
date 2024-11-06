using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject groundPrefab; // Префаб земли для начала и конца уровня
    public GameObject finishPrefab; // Префаб финишной зоны
    public int levelLength = 680;
    public int startZoneLength = 50;
    public int finishZoneLength = 30;
    public int miniGameLength = 100;
    private GameObject LvlOne;

    private List<System.Action<Vector3>> miniGames; // Список мини-игр
    private Vector3 spawnPosition = Vector3.zero;

    void Start()
    {
        LvlOne = new GameObject("Lvl One");
        InitializeMiniGames();
        GenerateLevel();
    }

    void InitializeMiniGames()
    {
        miniGames = new List<System.Action<Vector3>>
        {
            GenerateMiniGame1,
            GenerateMiniGame2,
            GenerateMiniGame3,
            // Добавьте другие мини-игры
        };
    }

    void GenerateLevel()
    {
        float blockWidth = 0.25f; // Ширина одного блока
        int totalBlocks = 680; // Общее количество блоков
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Начальная позиция спауна

        for (int i = 0; i < totalBlocks; i++)
        {
            Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            spawnPosition.x += blockWidth; // Смещение вправо на ширину блока (0.25)
        }
    }


    void GenerateFlatGround(int length)
    {
        for (int i = 0; i < length; i++)
        {
            Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            spawnPosition.x += 0.25f; // Смещение вправо на 0.25
        }
    }

    // Пример метода для первой мини-игры
    void GenerateMiniGame1(Vector3 startPosition)
    {
        int gapStart = 10;  // Начало первой пропасти
        int gapLength = 5;  // Длина пропасти
        int platformCount = 8;  // Количество платформ

        Vector3 platformPos = startPosition;
        platformPos.x += gapStart;

        for (int i = 0; i < platformCount; i++)
        {
            // Создаем платформу
            Instantiate(groundPrefab, platformPos, Quaternion.identity);

            // Смещаем каждую платформу на некоторое расстояние (для прыжков)
            platformPos.x += Random.Range(1.5f, 2.5f); // Измените 1.5f и 2.5f на нужные значения
            platformPos.y = Random.Range(1.5f, 3f);
        }
    }

    // Пример метода для второй мини-игры
    void GenerateMiniGame2(Vector3 startPosition)
    {
        // Логика для другой мини-игры
    }

    // Пример метода для третьей мини-игры
    void GenerateMiniGame3(Vector3 startPosition)
    {
        // Логика для третьей мини-игры
    }
}


