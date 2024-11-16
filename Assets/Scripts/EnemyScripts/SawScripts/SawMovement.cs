using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawMovement : MonoBehaviour
{
    public float moveDistance = 5f; // Дистанция, на которую пила движется в обе стороны
    public float speed = 2f;       // Скорость движения

    private Vector3 startPos;      // Начальная позиция пилы

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Вычисляем новую позицию пилы
        float offset = Mathf.PingPong(Time.time * speed, moveDistance * 2) - moveDistance;
        transform.position = startPos + new Vector3(offset, 0, 0);
    }
}
