using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelGenerator : MonoBehaviour
{
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    public int minTunnelLength = 12;
    public int maxTunnelLength = 20;
    public int minTunnelWidth = 3;
    public int maxTunnelWidth = 5;

    private Vector3[] directions = { Vector3.up, Vector3.right, Vector3.down, Vector3.left };

    public void GenerateTunnel(Vector3 start, Vector3 direction)
    {
        int tunnelLength = Random.Range(minTunnelLength, maxTunnelLength + 1);
        int tunnelWidth = Random.Range(minTunnelWidth, maxTunnelWidth + 1);

        Vector3 currentPosition = start;
        Vector3 currentDirection = direction;

        // Первые 5 блоков идут в заданном направлении
        for (int i = 0; i < 5 && i < tunnelLength; i++)
        {
            GenerateTunnelSegment(currentPosition, tunnelWidth);
            currentPosition += currentDirection; // Движемся в текущем направлении
        }

        // Оставшиеся блоки идут случайным образом
        for (int i = 5; i < tunnelLength; i++)
        {
            Vector3 newDirection;
            do
            {
                newDirection = directions[Random.Range(0, directions.Length)];
            } while (newDirection == -currentDirection); // Исключаем обратное направление

            currentDirection = newDirection;
            GenerateTunnelSegment(currentPosition, tunnelWidth);
            currentPosition += currentDirection; // Движемся в новом направлении
        }

        // Создаем двери в конце туннеля
        CreateTunnelEndDoor(currentPosition, currentDirection);
    }

    void GenerateTunnelSegment(Vector3 position, int width)
    {
        int halfWidth = width / 2;
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfWidth; y <= halfWidth; y++)
            {
                Vector3 floorPosition = new Vector3(position.x + x, position.y + y, 1);
                Instantiate(floorPrefab, floorPosition, Quaternion.identity, transform);

                // Создаем стены по краям туннеля
                if (x == -halfWidth || x == halfWidth || y == -halfWidth || y == halfWidth)
                {
                    Vector3 wallPosition = new Vector3(position.x + x, position.y + y, 0);
                    Instantiate(wallPrefab, wallPosition, Quaternion.identity, transform);
                }
            }
        }
    }

    void CreateTunnelEndDoor(Vector3 position, Vector3 direction)
    {
        // Делаем небольшое смещение для двери на основе направления
        Vector3 doorPosition = position + direction;
        Vector3 oppositeDirection = -direction;

        // Устанавливаем дверь в конце туннеля
        Instantiate(doorPrefab, doorPosition, Quaternion.Euler(0, 0, GetRotationForDirection(direction)), transform);

        // Устанавливаем дверь на противоположной стороне
        Vector3 oppositeDoorPosition = position + oppositeDirection;
        Instantiate(doorPrefab, oppositeDoorPosition, Quaternion.Euler(0, 0, GetRotationForDirection(oppositeDirection)), transform);
    }

    float GetRotationForDirection(Vector3 direction)
    {
        if (direction == Vector3.up)
            return 0;
        if (direction == Vector3.right)
            return 270;
        if (direction == Vector3.down)
            return 180;
        return 90;
    }
}