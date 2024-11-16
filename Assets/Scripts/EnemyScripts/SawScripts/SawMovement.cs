using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float moveDistance = 5f; // ���������, �� ������� ���� �������� � ��� �������
    public float speed = 2f;       // �������� ��������

    private Vector3 startPos;      // ��������� ������� ����

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // ��������� ����� ������� ����
        float offset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
