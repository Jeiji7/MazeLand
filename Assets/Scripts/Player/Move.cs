using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class Move : NetworkBehaviour
{
    public float speedPlayer = 5f; // Скорость игрока
    public float jumpForce = 5f; // Сила прыжка
    public LayerMask groundLayer; // Слой для земли
    public Transform groundCheck; // Позиция для проверки соприкосновения с землёй
    public float groundCheckRadius = 0.2f; // Радиус проверки соприкосновения с землёй

    private CapsuleCollider2D playerCollider;
    private Rigidbody2D rb;
    private Transform tr;
    private Vector2 movement;
    private bool isGrounded; // Флаг для проверки, на земле ли игрок
    [Header("Animation")]
    [SerializeField] private Animator MoveAnimation;
    void Start()
    {
        playerCollider = GetComponent<CapsuleCollider2D>();
        tr = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        //Time.timeScale = 2f;
        // Включаем интерполяцию для сглаживания движения
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
    }

    void Update()
    {
        if (!IsOwner)
            return;
        // Получаем направление движения по горизонтали
        float _directionHorizontal = Input.GetAxis("Horizontal");
        if (_directionHorizontal != 0)
        {
            MoveAnimation.SetBool("MoveRun", true);
        }
        else
        {
            MoveAnimation.SetBool("MoveRun", false);
        }
        // Изменяем масштаб для отражения персонажа влево или вправо
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);

        // Обновляем переменную движения с учётом времени между кадрами
        movement = new Vector2(_directionHorizontal, 0) * speedPlayer;

        // Проверяем, на земле ли игрок
        CheckGround();
        if (isGrounded)
            MoveAnimation.SetBool("MoveJump", false);
        else
        {
            MoveAnimation.SetBool("MoveRun", false);
            MoveAnimation.SetBool("MoveJump", true);
        }

        // Если игрок нажимает пробел и находится на земле, выполняем прыжок
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }

    }

    void FixedUpdate()
    {
        // Применяем движение к Rigidbody2D с учетом deltaTime для плавности
        rb.velocity = new Vector2(movement.x, rb.velocity.y);
    }

    void Jump()
    {
        // Применяем силу прыжка
        rb.velocity = new Vector2(jumpForce, jumpForce);
    }

    void CheckGround()
    {
        // Проверка, соприкасается ли игрок с землёй
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }
}
