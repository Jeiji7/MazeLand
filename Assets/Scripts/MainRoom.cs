using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRoom : MonoBehaviour
{
    public int sizeW;
    public int sizeH;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject cornerPrefab;

    public int roomCount = 3; // Количество комнат

    void Start()
    {
        GenerateRooms();
    }

    void GenerateRooms()
    {
        Vector3 roomOffset = Vector3.zero; // Изначальное смещение для первой комнаты
        for (int i = 0; i < roomCount; i++)
        {
            // Генерация случайных размеров комнаты
            sizeW = Random.Range(6, 16);
            sizeH = Random.Range(6, 16);

            // Генерация комнаты в рассчитанной позиции
            GenerateMatrix(roomOffset);

            // Вычисление смещения для следующей комнаты, чтобы они не накладывались друг на друга
            roomOffset += new Vector3(sizeW + Random.Range(3, 8), sizeH + Random.Range(3, 8), 0);
        }
    }

    void GenerateMatrix(Vector3 offset)
    {
        // Создаем пол по всей области
        CreateFloor(offset);

        // Размещение углов с поворотом
        PlaceCorner(0, 0, 270, offset); // Левый нижний угол
        PlaceCorner(sizeW - 1, 0, 180, offset); // Правый нижний угол
        PlaceCorner(0, sizeH - 1, 0, offset); // Левый верхний угол
        PlaceCorner(sizeW - 1, sizeH - 1, 90, offset); // Правый верхний угол

        // Размещение стен
        PlaceWalls(offset);

        // Создание дверей
        PlaceDoors(offset);
    }

    void CreateFloor(Vector3 offset)
    {
        for (int x = 0; x < sizeW; x++)
        {
            for (int y = 0; y < sizeH; y++)
            {
                Instantiate(floorPrefab, new Vector3(x, y, 0) + offset, Quaternion.identity, transform);
            }
        }
    }

    void PlaceCorner(int x, int y, float rotation, Vector3 offset)
    {
        Instantiate(cornerPrefab, new Vector3(x, y, 0) + offset, Quaternion.Euler(0, 0, rotation), transform);
    }

    void PlaceWalls(Vector3 offset)
    {
        // Верхняя и нижняя границы (без углов)
        for (int x = 1; x < sizeW - 1; x++)
        {
            Instantiate(wallPrefab, new Vector3(x, 0, 0) + offset, Quaternion.identity, transform); // Нижняя граница
            Instantiate(wallPrefab, new Vector3(x, sizeH - 1, 0) + offset, Quaternion.identity, transform); // Верхняя граница
        }

        // Левая и правая границы (без углов)
        for (int y = 1; y < sizeH - 1; y++)
        {
            Instantiate(wallPrefab, new Vector3(0, y, 0) + offset, Quaternion.identity, transform); // Левая граница
            Instantiate(wallPrefab, new Vector3(sizeW - 1, y, 0) + offset, Quaternion.identity, transform); // Правая граница
        }
    }

    void PlaceDoors(Vector3 offset)
    {
        int noDoorCount = 0; // Счетчик отсутствия дверей

        // Перечисляем каждую сторону: 0 - верх, 1 - право, 2 - низ, 3 - лево
        for (int side = 0; side < 4; side++)
        {
            int doorPosition;
            bool createDoor = Random.Range(0, 2) == 1;

            // Проверка условия обязательного создания двери после 3 отказов
            if (!createDoor)
            {
                noDoorCount++;
                if (noDoorCount >= 3)
                {
                    createDoor = true;
                    noDoorCount = 0;
                }
            }
            else
            {
                noDoorCount = 0;
            }

            if (createDoor)
            {
                switch (side)
                {
                    case 0: // Верхняя граница
                        doorPosition = Random.Range(1, sizeW - 2);
                        ClearAndReplaceWithDoor(doorPosition, sizeH - 1, 0, offset);
                        break;
                    case 1: // Правая граница
                        doorPosition = Random.Range(1, sizeH - 2);
                        ClearAndReplaceWithDoor(sizeW - 1, doorPosition, 270, offset);
                        break;
                    case 2: // Нижняя граница
                        doorPosition = Random.Range(1, sizeW - 2);
                        ClearAndReplaceWithDoor(doorPosition, 0, 180, offset);
                        break;
                    case 3: // Левая граница
                        doorPosition = Random.Range(1, sizeH - 2);
                        ClearAndReplaceWithDoor(0, doorPosition, 90, offset);
                        break;
                }
            }
        }
    }

    void ClearAndReplaceWithDoor(int x, int y, float rotation, Vector3 offset)
    {
        Vector3 position = new Vector3(x, y, 0) + offset;

        // Поиск и удаление объектов пола и стены в указанной позиции
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D col in colliders)
        {
            if (col.gameObject.CompareTag("Wall") || col.gameObject.CompareTag("Floor"))
            {
                Destroy(col.gameObject);
            }
        }

        // Установка двери
        Instantiate(doorPrefab, position, Quaternion.Euler(0, 0, rotation), transform);
    }
}
