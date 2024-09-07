using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeHoll : MonoBehaviour
{
    public int width = 40;
    public int height = 40;
    public GameObject wallPrefab;
    public GameObject floorPrefab;
    void Start()
    {
        GenerateMazeWorld();
    }

    void Update()
    {
        
    }

    public void GenerateMazeWorld()
    {
        bool[,] maze = new bool[width, height];
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                maze[i, j] = true;
            }
        }

        int currentX = (width/2) -20;
        int currentY = (height/2) -20;
        maze[currentX, currentY] = false;

        for (int i = 0; i < width * height; i++)
        {
            int direction = Random.Range(0, 4);
            switch (direction)
            {
                case 0: if (currentX > 0) currentX--; break; // Лево
                case 1: if (currentX < width - 1) currentX++; break; // Право
                case 2: if (currentY > 0) currentY--; break; // Вверх
                case 3: if (currentY < height - 1) currentY++; break; // Вниз
            }
            maze[currentX, currentY] = false; // Делать клетку путём
        }
        // Отрисовка
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y])
                {
                    Instantiate(wallPrefab, new Vector3(x -20, y - 20, -2), Quaternion.identity);
                }
                else
                {
                    Instantiate(floorPrefab, new Vector3(x -20, y -20, -2), Quaternion.identity);
                }
            }
        }
    }
}
