using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speedPlayer = 5f;
    private Rigidbody2D rb;
    private Transform tr;
    private Vector2 movement;

    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();

        // �������� ������������ ��� ����������� ��������
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // �������� ����������� �������� �� ����������� � ���������
        float _directionHorizontal = Input.GetAxis("Horizontal");
        float _directionVertical = Input.GetAxis("Vertical");

        // �������� ������� ��� ��������� ��������� ����� ��� ������
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);

        // ��������� ���������� �������� � ������ ������� ����� �������
        movement = new Vector2(_directionHorizontal, _directionVertical) * speedPlayer;
    }

    void FixedUpdate()
    {
        // ��������� �������� � Rigidbody2D � ������ deltaTime ��� ���������
        rb.velocity = movement * Time.fixedDeltaTime * 50f;
    }
}
