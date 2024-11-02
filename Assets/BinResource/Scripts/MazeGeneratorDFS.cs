using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorDFS : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject floor;  // ������ ����
    public GameObject wall;   // ������ �����

    private int[,] maze;

    private Stack<Vector2Int> stack = new Stack<Vector2Int>();
    private List<Vector2Int> directions = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start()
    {
        GenerateMaze();
        DrawMaze();
    }

    void GenerateMaze()
    {
        maze = new int[width, height];

        // �������������� ��� ������ ��� ����� (0 - �����, 1 - ���)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 0; // ��� ���������� ��� �����
            }
        }

        // ��������� �������
        Vector2Int currentCell = new Vector2Int(1, 1);
        stack.Push(currentCell);
        maze[currentCell.x, currentCell.y] = 1; // �������� ��� ���

        while (stack.Count > 0)
        {
            currentCell = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell);

            if (unvisitedNeighbors.Count > 0)
            {
                // �������� ���������� ������������� ������
                Vector2Int chosenCell = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];

                // ������� ������ ����� ������� ������� � ���������
                RemoveWall(currentCell, chosenCell);

                // ��������� � ������ � �������� ��� ��� ���
                maze[chosenCell.x, chosenCell.y] = 1;
                stack.Push(chosenCell);
            }
            else
            {
                // ���� ��� �������, ������������ �����
                stack.Pop();
            }
        }
    }

    List<Vector2Int> GetUnvisitedNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var direction in directions)
        {
            Vector2Int neighbor = cell + direction;
            // ���������, ��� ����� ��������� � �������� ����� � �������� ������ (0)
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    void RemoveWall(Vector2Int from, Vector2Int to)
    {
        // �������� �������� ����� ����� �������� � ������� �����
        Vector2Int between = (from + to) / 2;
        maze[between.x, between.y] = 1; // �������� �������� ��� ���
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x > 0 && pos.x < width - 1 && pos.y > 0 && pos.y < height - 1;
    }

    void DrawMaze()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (maze[x, y] == 1)
                {
                    // ��������� ���
                    Instantiate(floor, new Vector3(x, y, -1), Quaternion.identity);
                }
                else
                {
                    // ��������� �����
                    Instantiate(wall, new Vector3(x, y, -1), Quaternion.identity);
                }
            }
        }
    }
}
