using System.Collections;
using UnityEngine;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab; // Префаб камня
    public Transform throwPosition; // Позиция, из которой будет брошен камень
    public float throwForce = 10f; // Сила броска
    public float throwAngle = 45f; // Угол броска в градусах


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // Замените на нужную кнопку
        {
            ThrowRock();
        }
    }
    // Метод для броска камня
    public void ThrowRock()
    {
        // Создаём камень в позиции throwPosition
        GameObject rockInstance = Instantiate(rockPrefab, throwPosition.position, Quaternion.identity);

        // Получаем компонент Rigidbody2D камня
        Rigidbody2D rb = rockInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // Вычисляем угол броска в радианах
            float throwAngleRad = throwAngle * Mathf.Deg2Rad;

            // Определяем начальные скорости по x и y, чтобы камень двигался по параболической траектории
            Vector2 throwDirection = new Vector2(Mathf.Cos(throwAngleRad), Mathf.Sin(throwAngleRad));
            rb.velocity = throwDirection * throwForce;
        }
    }
}
