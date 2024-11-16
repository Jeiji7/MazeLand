using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SawTrigger : MonoBehaviour
{
    public float pushForceX = 15f;
    public float pushForceY = 10f; // ���� ������������ �����
    [Header("Animation")]
    public Animator sawAnim;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) // ���������, ��� ��� ������ �����
        {
            Rigidbody2D playerRb = collision.GetComponent<Rigidbody2D>();
            sawAnim.SetBool("woundPlayer", true);
            if (playerRb != null)
            {
                StartCoroutine(AttackPlayer());
                // ��������� ����������� ������������
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;

                // ��������� �������������� ������������
                Vector2 finalPush = new Vector2(pushDirection.x * pushForceX, pushForceY);

                // ��������� ���� ������������
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
