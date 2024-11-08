using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    public float speedPlayer = 5f; // �������� ������
    public float jumpForce = 5f; // ���� ������
    public LayerMask groundLayer; // ���� ��� �����
    public Transform groundCheck; // ������� ��� �������� ��������������� � �����
    public float groundCheckRadius = 0.2f; // ������ �������� ��������������� � �����

    private Rigidbody2D rb;
    private Transform tr;
    private Vector2 movement;
    private bool isGrounded; // ���� ��� ��������, �� ����� �� �����
    [Header("Animation")]
    [SerializeField] private Animator MoveAnimation;
    //[SerializeField] private bool AnimPlayerActive = false;
    void Start()
    {
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        //Time.timeScale = 2f;
        // �������� ������������ ��� ����������� ��������
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        // �������� ����������� �������� �� �����������
        float _directionHorizontal = Input.GetAxis("Horizontal");
        if (_directionHorizontal != 0)
        {
            MoveAnimation.SetBool("MoveRun", true);
        }
        else
        {
            MoveAnimation.SetBool("MoveRun", false);
            Debug.Log("Gavno");
        }
        // �������� ������� ��� ��������� ��������� ����� ��� ������
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);

        // ��������� ���������� �������� � ������ ������� ����� �������
        movement = new Vector2(_directionHorizontal, 0) * speedPlayer;

        // ���������, �� ����� �� �����
        CheckGround();
        if (isGrounded)
            MoveAnimation.SetBool("MoveJump", false);
        else
        {
            MoveAnimation.SetBool("MoveRun", false);
            MoveAnimation.SetBool("MoveJump", true);
        }

        // ���� ����� �������� ������ � ��������� �� �����, ��������� ������
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

    }

    void FixedUpdate()
    {
        // ��������� �������� � Rigidbody2D � ������ deltaTime ��� ���������
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    void Jump()
    {
        // ��������� ���� ������
        rb.velocity = new Vector2(rb.velocity.x * 1.4f, jumpForce);
    }

    void CheckGround()
    {
        // ��������, ������������� �� ����� � �����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
