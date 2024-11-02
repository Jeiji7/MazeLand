using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAlgorithm : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject floor;  // ������ ����
    public GameObject wall;   // ������ �����

    private int[,] grid;

    private List<Vector2Int> directions = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start()
    {
        grid = new int[width, height];

        // �������������� ��� ������ ��� ����� (-1 - �����)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = -1; // ��� ������ �� ��������� �����
            }
        }

        // ��������� ��������� �����
        Vector2Int start = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        grid[start.x, start.y] = 0;  // ��������� ����� - ���

        // ��������� �������� ��������� ���������������
        WavePropagation(start);
        DrawGrid();
    }

    void WavePropagation(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        grid[start.x, start.y] = 0; // �������� ��������� ������ ��� ����� ����

        while (queue.Count > 0)
        {
            Vector2Int currentCell = queue.Dequeue();

            // ������������ ����������� ��� �����������
            Shuffle(directions);

            foreach (var direction in directions)
            {
                Vector2Int neighbor = currentCell + direction;

                // ���������, ��� ����� � �������� ����� � ��� �� ������� (���� �����)
                if (IsInBounds(neighbor) && grid[neighbor.x, neighbor.y] == -1)
                {
                    // ������ ��� � �������� ������
                    grid[neighbor.x, neighbor.y] = 0;
                    queue.Enqueue(neighbor); // ��������� � ������� ��� ����������� ���������������

                    // �������� ������ �������
                    SurroundWithWalls(neighbor);
                }
            }
        }
    }

    // ������� ��� ������������� ������ ����������� (��������� �����������)
    void Shuffle(List<Vector2Int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Vector2Int temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void SurroundWithWalls(Vector2Int cell)
    {
        // ��������� �������� ������ ������ ������� ������
        foreach (var direction in directions)
        {
            Vector2Int wallPos = cell + direction;
            if (IsInBounds(wallPos) && grid[wallPos.x, wallPos.y] == -1)
            {
                // ���� �������� ������ � ��� ��� �� �����, ������ � ������
                grid[wallPos.x, wallPos.y] = -2;  // ���������� -2 ��� ����������� �����, ������� ����� ����� ����������
            }
        }
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }

    void DrawGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y] == 0)
                {
                    // ��������� ���
                    Instantiate(floor, new Vector3(x, y, -1), Quaternion.identity);
                }
                else if (grid[x, y] == -2)
                {
                    // ��������� �����
                    Instantiate(wall, new Vector3(x, y, -1), Quaternion.identity);
                }
            }
        }
    }
}
