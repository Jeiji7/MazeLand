using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class RockThrow : NetworkBehaviour
{
    [Header("Stouns")]
    [SerializeField] private GameObject rockPrefab; // Префаб камня
    [SerializeField] private bool rockStoune = true;
    [SerializeField] private GameObject oilrockPrefab; // Префаб черного замедляющего камня
    [SerializeField] private bool oilStoune = true;
    public Transform throwPosition; // Позиция, из которой будет брошен камень
    public float throwForce = 10f; // Сила броска
    public float throwAngle = 45f; // Угол броска в градусах
    private int numberAttack = 0;
    private Transform tr;
    [Header("Animation")]
    public Animator AttackAnimation;


    private void Start()
    {
        tr = GetComponent<Transform>();
    }


    void Update()
    {
        if (!IsOwner) return;
        float _directionHorizontal = Input.GetAxis("Horizontal");
        if (_directionHorizontal < 0)
            tr.localScale = new Vector3(-1, 1, 1);
        else if (_directionHorizontal > 0)
            tr.localScale = new Vector3(1, 1, 1);

        if (Input.GetKeyDown(KeyCode.K) && rockStoune) // Замените на нужную кнопку
        {
            rockStoune = false;
            numberAttack = 1;
            StartCoroutine(PlayerAttackRockStoune());
        }
        else if (Input.GetKeyDown(KeyCode.J) && oilStoune)
        {
            oilStoune = false;
            numberAttack = 2;
            StartCoroutine(PlayerAttackOilStoune());
        }
    }
    // Метод для броска камня

    [ServerRpc(RequireOwnership = false)]
    public void ThrowRockServerRpc()
    {
        ThrowRockClientRpc();
    }

    [ClientRpc]
    public void ThrowRockClientRpc()
    { 
        GameObject rockInstance;
        // Создаём камень в позиции throwPosition
        if (numberAttack == 1)
        {
            rockInstance = Instantiate(rockPrefab, throwPosition.position, Quaternion.identity);
        }
        else if (numberAttack == 2)
        {
            rockInstance = Instantiate(oilrockPrefab, throwPosition.position, Quaternion.identity);
        }
        else
        {
            rockInstance = null;
            Debug.LogError("Invalid attack number: " + numberAttack);
        }

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

        Destroy(rockInstance, 1.5f);
    }

    private void ThrowRock()
    {
        GameObject rockInstance;
        // Создаём камень в позиции throwPosition
        if (numberAttack == 1)
        {
            rockInstance = Instantiate(rockPrefab, throwPosition.position, Quaternion.identity);
        }
        else if (numberAttack == 2)
        {
            rockInstance = Instantiate(oilrockPrefab, throwPosition.position, Quaternion.identity);
        }
        else
        {
            rockInstance = null;
            Debug.LogError("Invalid attack number: " + numberAttack);
        }

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

        Destroy(rockInstance, 1.5f);
    }

    private IEnumerator PlayerAttackRockStoune()
    {
        AttackAnimation.SetBool("MoveRun", false);
        AttackAnimation.SetBool("MoveJump", false);
        AttackAnimation.SetBool("Attack", true);
        yield return new WaitForSeconds(0.2f);

        //ThrowRock();
        ThrowRockServerRpc();
        yield return new WaitForSeconds(0.5f);
        AttackAnimation.SetBool("Attack", false);
        yield return new WaitForSeconds(4.3f);
        rockStoune = true;
    }
    private IEnumerator PlayerAttackOilStoune()
    {
        AttackAnimation.SetBool("MoveRun", false);
        AttackAnimation.SetBool("MoveJump", false);
        AttackAnimation.SetBool("Attack", true);
        yield return new WaitForSeconds(0.2f);
        //ThrowRock();
        ThrowRockServerRpc();
        yield return new WaitForSeconds(0.5f);
        AttackAnimation.SetBool("Attack", false);
        yield return new WaitForSeconds(7.3f);
        oilStoune = true;
    }

}
