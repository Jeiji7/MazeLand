//using System.Collections.Generic;
//using UnityEngine;

//public class Room
//{
//    public Vector3 Position { get; private set; }
//    public int Width { get; private set; }
//    public int Height { get; private set; }
//    public List<Vector3> DoorPositions { get; private set; }

//    private RoomManager roomManager;

//    // Конструктор с передачей ссылки на RoomManager
//    public Room(Vector3 position, int width, int height, RoomManager roomManager)
//    {
//        Position = position;
//        Width = width;
//        Height = height;
//        DoorPositions = new List<Vector3>();
//        this.roomManager = roomManager;
//    }

//    public void CreateRoom()
//    {
//        CreateFloor();
//        PlaceCorners();
//        PlaceWalls();
//        PlaceDoors();
//    }

//    private void CreateFloor()
//    {
//        for (int x = 0; x < Width; x++)
//        {
//            for (int y = 0; y < Height; y++)
//            {
//                roomManager.Instantiate(roomManager.floorPrefab, Position + new Vector3(x, y, 1), Quaternion.identity);
//            }
//        }
//    }

//    private void PlaceCorners()
//    {
//        roomManager.Instantiate(roomManager.cornerPrefab, Position + new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 270));
//        roomManager.Instantiate(roomManager.cornerPrefab, Position + new Vector3(Width - 1, 0, 0), Quaternion.Euler(0, 0, 180));
//        roomManager.Instantiate(roomManager.cornerPrefab, Position + new Vector3(0, Height - 1, 0), Quaternion.Euler(0, 0, 0));
//        roomManager.Instantiate(roomManager.cornerPrefab, Position + new Vector3(Width - 1, Height - 1, 0), Quaternion.Euler(0, 0, 90));
//    }

//    private void PlaceWalls()
//    {
//        // Верхняя и нижняя границы (без углов)
//        for (int x = 1; x < Width - 1; x++)
//        {
//            roomManager.Instantiate(roomManager.wallPrefab, Position + new Vector3(x, 0, 0), Quaternion.identity);
//            roomManager.Instantiate(roomManager.wallPrefab, Position + new Vector3(x, Height - 1, 0), Quaternion.identity);
//        }

//        // Левая и правая границы (без углов)
//        for (int y = 1; y < Height - 1; y++)
//        {
//            roomManager.Instantiate(roomManager.wallPrefab, Position + new Vector3(0, y, 0), Quaternion.identity);
//            roomManager.Instantiate(roomManager.wallPrefab, Position + new Vector3(Width - 1, y, 0), Quaternion.identity);
//        }
//    }

//    private void PlaceDoors()
//    {
//        int noDoorCount = 0;

//        for (int side = 0; side < 4; side++)
//        {
//            int doorPosition;
//            bool createDoor = Random.Range(0, 2) == 1;

//            if (!createDoor)
//            {
//                noDoorCount++;
//                if (noDoorCount >= 3)
//                {
//                    createDoor = true;
//                    noDoorCount = 0;
//                }
//            }
//            else
//            {
//                noDoorCount = 0;
//            }

//            if (createDoor)
//            {
//                switch (side)
//                {
//                    case 0: // Верхняя граница
//                        doorPosition = Random.Range(1, Width - 2);
//                        CreateDoor(doorPosition, Height - 1, 0);
//                        break;
//                    case 1: // Правая граница
//                        doorPosition = Random.Range(1, Height - 2);
//                        CreateDoor(Width - 1, doorPosition, 270);
//                        break;
//                    case 2: // Нижняя граница
//                        doorPosition = Random.Range(1, Width - 2);
//                        CreateDoor(doorPosition, 0, 180);
//                        break;
//                    case 3: // Левая граница
//                        doorPosition = Random.Range(1, Height - 2);
//                        CreateDoor(0, doorPosition, 90);
//                        break;
//                }
//            }
//        }
//    }

//    private void CreateDoor(int x, int y, float rotation)
//    {
//        Vector3 position = Position + new Vector3(x, y, 0);
//        roomManager.Instantiate(roomManager.doorPrefab, position, Quaternion.Euler(0, 0, rotation));
//        DoorPositions.Add(position);
//    }
//}
