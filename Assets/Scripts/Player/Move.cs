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

        // Включаем интерполяцию для сглаживания движения
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // Получаем направление движения по горизонтали и вертикали
        float _directionHorizontal = Input.GetAxis("Horizontal");
        float _directionVertical = Input.GetAxis("Vertical");

        // Изменяем масштаб для отражения персонажа влево или вправо
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);

        // Обновляем переменную движения с учётом времени между кадрами
        movement = new Vector2(_directionHorizontal, _directionVertical) * speedPlayer;
    }

    void FixedUpdate()
    {
        // Применяем движение к Rigidbody2D с учетом deltaTime для плавности
        rb.velocity = movement * Time.fixedDeltaTime * 50f;
    }
}
