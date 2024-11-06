using System.Collections;
using UnityEngine;

public class RockThrow : MonoBehaviour
{
    public GameObject rockPrefab; // ������ �����
    public Transform throwPosition; // �������, �� ������� ����� ������ ������
    public float throwForce = 10f; // ���� ������
    public float throwAngle = 45f; // ���� ������ � ��������


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) // �������� �� ������ ������
        {
            ThrowRock();
        }
    }
    // ����� ��� ������ �����
    public void ThrowRock()
    {
        // ������ ������ � ������� throwPosition
        GameObject rockInstance = Instantiate(rockPrefab, throwPosition.position, Quaternion.identity);

        // �������� ��������� Rigidbody2D �����
        Rigidbody2D rb = rockInstance.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            // ��������� ���� ������ � ��������
            float throwAngleRad = throwAngle * Mathf.Deg2Rad;

            // ���������� ��������� �������� �� x � y, ����� ������ �������� �� �������������� ����������
            Vector2 throwDirection = new Vector2(Mathf.Cos(throwAngleRad), Mathf.Sin(throwAngleRad));
            rb.velocity = throwDirection * throwForce;
        }
    }
}
