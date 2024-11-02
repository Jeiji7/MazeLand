using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorDFS : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject floor;  // Префаб пола
    public GameObject wall;   // Префаб стены

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

        // Инициализируем все клетки как стены (0 - стена, 1 - пол)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                maze[x, y] = 0; // Все начинается как стены
            }
        }

        // Стартовая позиция
        Vector2Int currentCell = new Vector2Int(1, 1);
        stack.Push(currentCell);
        maze[currentCell.x, currentCell.y] = 1; // Помечаем как пол

        while (stack.Count > 0)
        {
            currentCell = stack.Peek();
            List<Vector2Int> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell);

            if (unvisitedNeighbors.Count > 0)
            {
                // Выбираем случайного непосещённого соседа
                Vector2Int chosenCell = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];

                // Убираем стенку между текущей клеткой и выбранной
                RemoveWall(currentCell, chosenCell);

                // Переходим к соседу и помечаем его как пол
                maze[chosenCell.x, chosenCell.y] = 1;
                stack.Push(chosenCell);
            }
            else
            {
                // Если нет соседей, возвращаемся назад
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
            // Проверяем, что сосед находится в границах карты и является стеной (0)
            if (IsInBounds(neighbor) && maze[neighbor.x, neighbor.y] == 0)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    void RemoveWall(Vector2Int from, Vector2Int to)
    {
        // Выбираем середину между двумя клетками и убираем стену
        Vector2Int between = (from + to) / 2;
        maze[between.x, between.y] = 1; // Помечаем середину как пол
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
                    // Размещаем пол
                    Instantiate(floor, new Vector3(x, y, -1), Quaternion.identity);
                }
                else
                {
                    // Размещаем стены
                    Instantiate(wall, new Vector3(x, y, -1), Quaternion.identity);
                }
            }
        }
    }
}
