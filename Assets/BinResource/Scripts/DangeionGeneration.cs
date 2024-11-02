using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int width = 50;
    public int height = 50;
    public int roomCount = 5;
    public int minRoomSize = 4;
    public int maxRoomSize = 8;

    public GameObject floorPrefab;  // Префаб пола
    public GameObject wallPrefab;   // Префаб стены
    public GameObject blackBlockPrefab; // Префаб чёрного блока
    public GameObject Corner;
    public GameObject Player;
    public GameObject CameraPlayer;

    private int[,] grid;

    class Room
    {
        public int x, y, width, height;

        public Room(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        public bool Intersects(Room other)
        {
            return !(x + width < other.x || other.x + other.width < x || y + height < other.y || other.y + other.height < y);
        }
        public Vector3 GetCenter()
        {
            return new Vector3(x + width / 2, y + height / 2, -2);
        }
        public Vector3 GetCenterCamera()
        {
            return new Vector3(x + width / 2, y + height / 2, -10);
        }
    }

    void Start()
    {
        grid = new int[width, height];

        // Инициализация карты как стены
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = -1;  // -1 означает стена
            }
        }

        List<Room> rooms = GenerateRooms();
        CreateCorridors(rooms);
        AddBlackBlocks();
        AddCorners();
        DrawMap();

        // Спауним персонажа в случайной комнате
        SpawnPlayerInRandomRoom(rooms);
    }


    

    // Генерация комнат
    List<Room> GenerateRooms()
    {
        List<Room> rooms = new List<Room>();

        for (int i = 0; i < roomCount; i++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);
            int roomX = Random.Range(1, width - roomWidth - 1); // Не ставим комнату на границу
            int roomY = Random.Range(1, height - roomHeight - 1);

            Room newRoom = new Room(roomX, roomY, roomWidth, roomHeight);

            bool intersects = false;
            foreach (Room room in rooms)
            {
                if (newRoom.Intersects(room))
                {
                    intersects = true;
                    break;
                }
            }

            if (!intersects)
            {
                rooms.Add(newRoom);

                // Заполняем полом
                for (int x = newRoom.x; x < newRoom.x + newRoom.width; x++)
                {
                    for (int y = newRoom.y; y < newRoom.y + newRoom.height; y++)
                    {
                        grid[x, y] = 0;  // 0 означает пол
                    }
                }
            }
        }

        return rooms;
    }

    // Создание коридоров
    void CreateCorridors(List<Room> rooms)
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            Room roomA = rooms[i];
            Room roomB = rooms[i + 1];

            int startX = roomA.x + roomA.width / 2;
            int startY = roomA.y + roomA.height / 2;
            int endX = roomB.x + roomB.width / 2;
            int endY = roomB.y + roomB.height / 2;

            while (startX != endX)
            {
                grid[startX, startY] = 0; // Пол
                startX += (startX < endX) ? 1 : -1;
            }

            while (startY != endY)
            {
                grid[startX, startY] = 0; // Пол
                startY += (startY < endY) ? 1 : -1;
            }
        }
    }

    // Добавляем углы в местах поворота

    // Добавление чёрных блоков вокруг стен
    void AddBlackBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == -1) // Если это стена
                {
                    bool hasFloorNearby = false;

                    // Проверяем 8 соседей вокруг текущей клетки
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            // Пропускаем саму клетку (x, y)
                            if (dx == 0 && dy == 0) continue;

                            int newX = x + dx;
                            int newY = y + dy;

                            if (!IsInBounds(newX, newY)) continue; // Пропускаем клетки за границами

                            // Если рядом есть пол (значение 0), отмечаем это
                            if (grid[newX, newY] == 0)
                            {
                                hasFloorNearby = true;
                                break; // Достаточно найти один пол
                            }
                        }

                        if (hasFloorNearby) break; // Если нашли пол, прекращаем дальнейшую проверку
                    }

                    // Если рядом с клеткой нет пола, превращаем её в чёрный блок
                    if (!hasFloorNearby)
                    {
                        grid[x, y] = -2; // Устанавливаем чёрный блок
                    }
                }
            }
        }
    }

    void AddCorners()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == -1) // Это стена
                {
                    bool horizontalSame = IsInBounds(x - 1, y) && IsInBounds(x + 1, y) && (grid[x - 1, y] == grid[x + 1, y]);
                    bool verticalSame = IsInBounds(x, y - 1) && IsInBounds(x, y + 1) && (grid[x, y - 1] == grid[x, y + 1]);

                    // Проверяем соседей, чтобы рядом не было угла (-3)
                    bool hasCornerNeighbor =
                        (IsInBounds(x - 1, y) && grid[x - 1, y] == -3) ||
                        (IsInBounds(x + 1, y) && grid[x + 1, y] == -3) ||
                        (IsInBounds(x, y - 1) && grid[x, y - 1] == -3) ||
                        (IsInBounds(x, y + 1) && grid[x, y + 1] == -3);

                    // Если по горизонтали или вертикали соседи совпадают, или есть соседний угол, оставляем стену
                    if (horizontalSame || verticalSame || hasCornerNeighbor)
                    {
                        continue; // Это не угол, продолжаем как стена
                    }

                    // Это потенциальный угол, проверяем диагонали
                    Quaternion rotation = Quaternion.identity;

                    // Проверяем левый нижний диагональный угол
                    if (IsInBounds(x - 1, y - 1) && grid[x - 1, y - 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, -90); // Поворот на -90°
                    }
                    // Проверяем правый нижний диагональный угол
                    else if (IsInBounds(x + 1, y - 1) && grid[x + 1, y - 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 0); // Поворот на 0°
                    }
                    // Проверяем правый верхний диагональный угол
                    else if (IsInBounds(x + 1, y + 1) && grid[x + 1, y + 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 90); // Поворот на 90°
                    }
                    // Проверяем левый верхний диагональный угол
                    else if (IsInBounds(x - 1, y + 1) && grid[x - 1, y + 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 180); // Поворот на 180°
                    }



                    // Устанавливаем угол и отмечаем его как угол в сетке
                    grid[x, y] = -3; // Отмечаем как угол
                    Instantiate(Corner, new Vector3(x, y, 0), rotation);
                }
            }
        }
    }


    void SpawnPlayerInRandomRoom(List<Room> rooms)
    {
        int randomRoomIndex = Random.Range(0, rooms.Count); // Выбираем случайную комнату
        Room randomRoom = rooms[randomRoomIndex];
        Vector3 spawnPosition = randomRoom.GetCenter(); // Получаем центр комнаты
        Vector3 spawnPositionCamera = randomRoom.GetCenterCamera(); // Получаем центр комнаты
        GameObject playerInstance = Instantiate(Player, spawnPosition, Quaternion.identity);

        // Спауним виртуальную камеру
        GameObject cameraInstance = Instantiate(CameraPlayer, spawnPositionCamera, Quaternion.identity);

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
    }

    // Проверка границ карты
    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // Отрисовка карты
    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0)
                {
                    // Размещаем пол
                    Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (grid[x, y] == -1)
                {
                    // Размещаем стены
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (grid[x, y] == -2)
                {
                    // Размещаем чёрные блоки
                    Instantiate(blackBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    Debug.Log("Чёрный блок на позиции: " + x + "," + y);
                }
            }
        }
    }
}
