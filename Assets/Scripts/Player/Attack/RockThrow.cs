using System.Collections;
using System.Linq;
using UnityEngine;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab; // Префаб камня
    public Transform throwPosition; // Позиция, из которой будет брошен камень
    public float throwForce = 10f; // Сила броска
    public float throwAngle = 45f; // Угол броска в градусах
    private Transform tr;
    [Header("Animation")]
    public Animator AttackAnimation;


    private void Start()
    {
        tr = GetComponent<Transform>();
    }


    void Update()
    {
        float _directionHorizontal = Input.GetAxis("Horizontal");
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);
        if (Input.GetKeyDown(KeyCode.K)) // Замените на нужную кнопку
        {
            StartCoroutine(PlayerAttack());
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
            Vector2 throwDirection = new Vector2(tr.localScale.x * Mathf.Cos(throwAngleRad), Mathf.Sin(throwAngleRad));
            rb.velocity = throwDirection * throwForce;
        }
    }

    private IEnumerator PlayerAttack()
    {
        AttackAnimation.SetBool("MoveRun", false);
        AttackAnimation.SetBool("MoveJump", false);
        AttackAnimation.SetBool("Attack", true);
        yield return new WaitForSeconds(0.2f);
        ThrowRock();
        yield return new WaitForSeconds(0.5f);
        AttackAnimation.SetBool("Attack", false);
    }
}
