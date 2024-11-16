using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrigger : MonoBehaviour
{
    public float pushForceX = 15f;
    public float pushForceY = 10f; // Сила отталкивания героя
    [Header("Animation")]
    public Animator sawAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // Проверяем, что это именно герой
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            sawAnim.SetBool("woundPlayer", true);
            if (playerRb != null)
            {
                StartCoroutine(AttackPlayer());
                // Вычисляем направление отталкивания
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

                // Усиливаем горизонтальное отталкивание
                Vector2 finalPush = new Vector2(pushDirection.x * pushForceX, pushForceY);

                // Применяем силу отталкивания
                playerRb.AddForce(finalPush, ForceMode2D.Impulse);
            }
        }
    }

    private IEnumerator AttackPlayer()
    {
        yield return new WaitForSeconds(2f);
        sawAnim.SetBool("woundPlayer", false);
    }
}
