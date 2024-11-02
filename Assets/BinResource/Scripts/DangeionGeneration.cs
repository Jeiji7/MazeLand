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

    public GameObject floorPrefab;  // ������ ����
    public GameObject wallPrefab;   // ������ �����
    public GameObject blackBlockPrefab; // ������ ������� �����
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

        // ������������� ����� ��� �����
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = -1;  // -1 �������� �����
            }
        }

        List<Room> rooms = GenerateRooms();
        CreateCorridors(rooms);
        AddBlackBlocks();
        AddCorners();
        DrawMap();

        // ������� ��������� � ��������� �������
        SpawnPlayerInRandomRoom(rooms);
    }


    

    // ��������� ������
    List<Room> GenerateRooms()
    {
        List<Room> rooms = new List<Room>();

        for (int i = 0; i < roomCount; i++)
        {
            int roomWidth = Random.Range(minRoomSize, maxRoomSize);
            int roomHeight = Random.Range(minRoomSize, maxRoomSize);
            int roomX = Random.Range(1, width - roomWidth - 1); // �� ������ ������� �� �������
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

                // ��������� �����
                for (int x = newRoom.x; x < newRoom.x + newRoom.width; x++)
                {
                    for (int y = newRoom.y; y < newRoom.y + newRoom.height; y++)
                    {
                        grid[x, y] = 0;  // 0 �������� ���
                    }
                }
            }
        }

        return rooms;
    }

    // �������� ���������
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
                grid[startX, startY] = 0; // ���
                startX += (startX < endX) ? 1 : -1;
            }

            while (startY != endY)
            {
                grid[startX, startY] = 0; // ���
                startY += (startY < endY) ? 1 : -1;
            }
        }
    }

    // ��������� ���� � ������ ��������

    // ���������� ������ ������ ������ ����
    void AddBlackBlocks()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == -1) // ���� ��� �����
                {
                    bool hasFloorNearby = false;

                    // ��������� 8 ������� ������ ������� ������
                    for (int dx = -1; dx <= 1; dx++)
                    {
                        for (int dy = -1; dy <= 1; dy++)
                        {
                            // ���������� ���� ������ (x, y)
                            if (dx == 0 && dy == 0) continue;

                            int newX = x + dx;
                            int newY = y + dy;

                            if (!IsInBounds(newX, newY)) continue; // ���������� ������ �� ���������

                            // ���� ����� ���� ��� (�������� 0), �������� ���
                            if (grid[newX, newY] == 0)
                            {
                                hasFloorNearby = true;
                                break; // ���������� ����� ���� ���
                            }
                        }

                        if (hasFloorNearby) break; // ���� ����� ���, ���������� ���������� ��������
                    }

                    // ���� ����� � ������� ��� ����, ���������� � � ������ ����
                    if (!hasFloorNearby)
                    {
                        grid[x, y] = -2; // ������������� ������ ����
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
                if (grid[x, y] == -1) // ��� �����
                {
                    bool horizontalSame = IsInBounds(x - 1, y) && IsInBounds(x + 1, y) && (grid[x - 1, y] == grid[x + 1, y]);
                    bool verticalSame = IsInBounds(x, y - 1) && IsInBounds(x, y + 1) && (grid[x, y - 1] == grid[x, y + 1]);

                    // ��������� �������, ����� ����� �� ���� ���� (-3)
                    bool hasCornerNeighbor =
                        (IsInBounds(x - 1, y) && grid[x - 1, y] == -3) ||
                        (IsInBounds(x + 1, y) && grid[x + 1, y] == -3) ||
                        (IsInBounds(x, y - 1) && grid[x, y - 1] == -3) ||
                        (IsInBounds(x, y + 1) && grid[x, y + 1] == -3);

                    // ���� �� ����������� ��� ��������� ������ ���������, ��� ���� �������� ����, ��������� �����
                    if (horizontalSame || verticalSame || hasCornerNeighbor)
                    {
                        continue; // ��� �� ����, ���������� ��� �����
                    }

                    // ��� ������������� ����, ��������� ���������
                    Quaternion rotation = Quaternion.identity;

                    // ��������� ����� ������ ������������ ����
                    if (IsInBounds(x - 1, y - 1) && grid[x - 1, y - 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, -90); // ������� �� -90�
                    }
                    // ��������� ������ ������ ������������ ����
                    else if (IsInBounds(x + 1, y - 1) && grid[x + 1, y - 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 0); // ������� �� 0�
                    }
                    // ��������� ������ ������� ������������ ����
                    else if (IsInBounds(x + 1, y + 1) && grid[x + 1, y + 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 90); // ������� �� 90�
                    }
                    // ��������� ����� ������� ������������ ����
                    else if (IsInBounds(x - 1, y + 1) && grid[x - 1, y + 1] == 0)
                    {
                        rotation = Quaternion.Euler(0, 0, 180); // ������� �� 180�
                    }



                    // ������������� ���� � �������� ��� ��� ���� � �����
                    grid[x, y] = -3; // �������� ��� ����
                    Instantiate(Corner, new Vector3(x, y, 0), rotation);
                }
            }
        }
    }


    void SpawnPlayerInRandomRoom(List<Room> rooms)
    {
        int randomRoomIndex = Random.Range(0, rooms.Count); // �������� ��������� �������
        Room randomRoom = rooms[randomRoomIndex];
        Vector3 spawnPosition = randomRoom.GetCenter(); // �������� ����� �������
        Vector3 spawnPositionCamera = randomRoom.GetCenterCamera(); // �������� ����� �������
        GameObject playerInstance = Instantiate(Player, spawnPosition, Quaternion.identity);

        // ������� ����������� ������
        GameObject cameraInstance = Instantiate(CameraPlayer, spawnPositionCamera, Quaternion.identity);

        // ������������� ���������� ������ �� �������
        CinemachineVirtualCamera virtualCam = cameraInstance.GetComponent<CinemachineVirtualCamera>();

        if (virtualCam != null)
        {
            virtualCam.Follow = playerInstance.transform; // ������ ����� ������� �� �������
            virtualCam.LookAt = playerInstance.transform; // ������ ����� ������� �� �������
        }
        else
        {
            Debug.LogError("VirtualCamera ��������� �� ������ �� ������� CameraPlayer!");
        }
    }

    // �������� ������ �����
    bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    // ��������� �����
    void DrawMap()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0)
                {
                    // ��������� ���
                    Instantiate(floorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (grid[x, y] == -1)
                {
                    // ��������� �����
                    Instantiate(wallPrefab, new Vector3(x, y, 0), Quaternion.identity);
                }
                else if (grid[x, y] == -2)
                {
                    // ��������� ������ �����
                    Instantiate(blackBlockPrefab, new Vector3(x, y, 0), Quaternion.identity);
                    Debug.Log("׸���� ���� �� �������: " + x + "," + y);
                }
            }
        }
    }
}
