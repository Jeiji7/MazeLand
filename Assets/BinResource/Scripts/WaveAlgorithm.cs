using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAlgorithm : MonoBehaviour
{
    public int width = 10;
    public int height = 10;
    public GameObject floor;  // Префаб пола
    public GameObject wall;   // Префаб стены

    private int[,] grid;

    private List<Vector2Int> directions = new List<Vector2Int>
    {
        Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
    };

    void Start()
    {
        grid = new int[width, height];

        // Инициализируем все клетки как стены (-1 - стена)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = -1; // Все клетки по умолчанию стены
            }
        }

        // Случайная стартовая точка
        Vector2Int start = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        grid[start.x, start.y] = 0;  // Стартовая точка - пол

        // Запускаем алгоритм волнового распространения
        WavePropagation(start);
        DrawGrid();
    }

    void WavePropagation(Vector2Int start)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(start);
        grid[start.x, start.y] = 0; // Помечаем стартовую клетку как часть пути

        while (queue.Count > 0)
        {
            Vector2Int currentCell = queue.Dequeue();

            // Перемешиваем направления для случайности
            Shuffle(directions);

            foreach (var direction in directions)
            {
                Vector2Int neighbor = currentCell + direction;

                // Проверяем, что сосед в пределах карты и ещё не посещён (пока стена)
                if (IsInBounds(neighbor) && grid[neighbor.x, neighbor.y] == -1)
                {
                    // Создаём пол в соседней клетке
                    grid[neighbor.x, neighbor.y] = 0;
                    queue.Enqueue(neighbor); // Добавляем в очередь для дальнейшего распространения

                    // Окружаем клетки стенами
                    SurroundWithWalls(neighbor);
                }
            }
        }
    }

    // Функция для перемешивания списка направлений (добавляет случайность)
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
        // Проверяем соседние клетки вокруг текущей клетки
        foreach (var direction in directions)
        {
            Vector2Int wallPos = cell + direction;
            if (IsInBounds(wallPos) && grid[wallPos.x, wallPos.y] == -1)
            {
                // Если соседняя клетка — это ещё не стена, делаем её стеной
                grid[wallPos.x, wallPos.y] = -2;  // Используем -2 для обозначения стены, которую нужно будет отрисовать
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
                    // Размещаем пол
                    Instantiate(floor, new Vector3(x, y, -1), Quaternion.identity);
                }
                else if (grid[x, y] == -2)
                {
                    // Размещаем стены
                    Instantiate(wall, new Vector3(x, y, -1), Quaternion.identity);
                }
            }
        }
    }
}
