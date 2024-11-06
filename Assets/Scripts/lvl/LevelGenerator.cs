using UnityEngine;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    public GameObject groundPrefab; // ������ ����� ��� ������ � ����� ������
    public GameObject finishPrefab; // ������ �������� ����
    public int levelLength = 680;
    public int startZoneLength = 50;
    public int finishZoneLength = 30;
    public int miniGameLength = 100;
    private GameObject LvlOne;

    private List<System.Action<Vector3>> miniGames; // ������ ����-���
    private Vector3 spawnPosition = Vector3.zero;

    void Start()
    {
        LvlOne = new GameObject("Lvl One");
        InitializeMiniGames();
        GenerateLevel();
    }

    void InitializeMiniGames()
    {
        miniGames = new List<System.Action<Vector3>>
        {
            GenerateMiniGame1,
            GenerateMiniGame2,
            GenerateMiniGame3,
            // �������� ������ ����-����
        };
    }

    void GenerateLevel()
    {
        float blockWidth = 0.25f; // ������ ������ �����
        int totalBlocks = 680; // ����� ���������� ������
        Vector3 spawnPosition = new Vector3(0, 0, 0); // ��������� ������� ������

        for (int i = 0; i < totalBlocks; i++)
        {
            Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            spawnPosition.x += blockWidth; // �������� ������ �� ������ ����� (0.25)
        }
    }


    void GenerateFlatGround(int length)
    {
        for (int i = 0; i < length; i++)
        {
            Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
            spawnPosition.x += 0.25f; // �������� ������ �� 0.25
        }
    }

    // ������ ������ ��� ������ ����-����
    void GenerateMiniGame1(Vector3 startPosition)
    {
        int gapStart = 10;  // ������ ������ ��������
        int gapLength = 5;  // ����� ��������
        int platformCount = 8;  // ���������� ��������

        Vector3 platformPos = startPosition;
        platformPos.x += gapStart;

        for (int i = 0; i < platformCount; i++)
        {
            // ������� ���������
            Instantiate(groundPrefab, platformPos, Quaternion.identity);

            // ������� ������ ��������� �� ��������� ���������� (��� �������)
            platformPos.x += Random.Range(1.5f, 2.5f); // �������� 1.5f � 2.5f �� ������ ��������
            platformPos.y = Random.Range(1.5f, 3f);
        }
    }

    // ������ ������ ��� ������ ����-����
    void GenerateMiniGame2(Vector3 startPosition)
    {
        // ������ ��� ������ ����-����
    }

    // ������ ������ ��� ������� ����-����
    void GenerateMiniGame3(Vector3 startPosition)
    {
        // ������ ��� ������� ����-����
    }
}


