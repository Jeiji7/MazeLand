using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Move : NetworkBehaviour
{
    public float speedPlayer = 5f; // �������� ������
    public float jumpForce = 5f; // ���� ������
    public LayerMask groundLayer; // ���� ��� �����
    public Transform groundCheck; // ������� ��� �������� ��������������� � �����
    public float groundCheckRadius = 0.2f; // ������ �������� ��������������� � �����

    private CapsuleCollider2D playerCollider;
    private Rigidbody2D rb;
    private Transform tr;
    private Vector2 movement;
    private bool isGrounded; // ���� ��� ��������, �� ����� �� �����
    [Header("Animation")]
    [SerializeField] private Animator MoveAnimation;
    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        //Time.timeScale = 2f;
        // �������� ������������ ��� ����������� ��������
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        // �������� ����������� �������� �� �����������
        float _directionHorizontal = Input.GetAxis("Horizontal");
        if (_directionHorizontal != 0)
        {
            MoveAnimation.SetBool("MoveRun", true);
        }
        else
        {
            MoveAnimation.SetBool("MoveRun", false);
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
        rb.velocity = new Vector2(jumpForce, jumpForce);
    }

    void CheckGround()
    {
        // ��������, ������������� �� ����� � �����
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
