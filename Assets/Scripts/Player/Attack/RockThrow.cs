//using System.Collections;
//using System.Linq;
//using Unity.Netcode;
//using UnityEngine;

//public class RockThrow : NetworkBehaviour
//{
//    [Header("Stouns")]
//    [SerializeField] private GameObject rockPrefab; // Префаб камня
//    [SerializeField] private bool rockStoune = true;
//    [SerializeField] private GameObject oilrockPrefab; // Префаб черного замедляющего камня
//    [SerializeField] private bool oilStoune = true;
//    public Transform throwPosition; // Позиция, из которой будет брошен камень
//    public float throwForce = 10f; // Сила броска
//    public float throwAngle = 45f; // Угол броска в градусах
//    private int numberAttack = 0;
//    private Transform tr;
//    [Header("Animation")]
//    public Animator AttackAnimation;


//    private void Start()
//    {
//        tr = GetComponent<Transform>();
//    }


//    void Update()
//    {
//        if (!IsOwner) return;
//        float _directionHorizontal = Input.GetAxis("Horizontal");
//        if (_directionHorizontal < 0)
//            tr.localScale = new Vector3(-1, 1, 1);
//        else if (_directionHorizontal > 0)
//            tr.localScale = new Vector3(1, 1, 1);

//        if (Input.GetKeyDown(KeyCode.K) && rockStoune) // Замените на нужную кнопку
//        {
//            rockStoune = false;
//            numberAttack = 1;
//            StartCoroutine(PlayerAttackRockStoune());
//        }
//        else if (Input.GetKeyDown(KeyCode.J) && oilStoune)
//        {
//            oilStoune = false;
//            numberAttack = 2;
//            StartCoroutine(PlayerAttackOilStoune());
//        }
//    }
//    // Метод для броска камня

//    [ServerRpc(RequireOwnership = false)]
//    public void ThrowRockServerRpc()
//    {
//        ThrowRockClientRpc();
//    }

//    [ClientRpc(RequireOwnership = false)]
//    public void ThrowRockClientRpc()
//    { 
//        Transform rockInstance;
//        // Создаём камень в позиции throwPosition
//        if (numberAttack == 1)
//        {
//            rockInstance = Instantiate(rockPrefab.transform, throwPosition.position, Quaternion.identity);
//        }
//        else if (numberAttack == 2)
//        {
//            rockInstance = Instantiate(oilrockPrefab.transform, throwPosition.position, Quaternion.identity);
//        }
//        else
//        {
//            rockInstance = null;
//            Debug.LogError("Invalid attack number: " + numberAttack);
//        }
//        rockInstance.GetComponent<NetworkObject>().Spawn();
//        // Получаем компонент Rigidbody2D камня
//        Rigidbody2D rb = rockInstance.GetComponent<Rigidbody2D>();

//        if (rb != null)
//        {
//            // Вычисляем угол броска в радианах
//            float throwAngleRad = throwAngle * Mathf.Deg2Rad;

//            // Определяем начальные скорости по x и y, чтобы камень двигался по параболической траектории
//            Vector2 throwDirection = new Vector2(tr.localScale.x * Mathf.Cos(throwAngleRad), Mathf.Sin(throwAngleRad));
//            rb.velocity = throwDirection * throwForce;
//        }

//        Destroy(rockInstance, 2f);
//    }


//    private IEnumerator PlayerAttackRockStoune()
//    {
//        AttackAnimation.SetBool("MoveRun", false);
//        AttackAnimation.SetBool("MoveJump", false);
//        AttackAnimation.SetBool("Attack", true);
//        yield return new WaitForSeconds(0.2f);

//        //ThrowRock();
//        ThrowRockServerRpc();
//        yield return new WaitForSeconds(0.5f);
//        AttackAnimation.SetBool("Attack", false);
//        yield return new WaitForSeconds(4.3f);
//        rockStoune = true;
//    }
//    private IEnumerator PlayerAttackOilStoune()
//    {
//        AttackAnimation.SetBool("MoveRun", false);
//        AttackAnimation.SetBool("MoveJump", false);
//        AttackAnimation.SetBool("Attack", true);
//        yield return new WaitForSeconds(0.2f);
//        //ThrowRock();
//        ThrowRockServerRpc();
//        yield return new WaitForSeconds(0.5f);
//        AttackAnimation.SetBool("Attack", false);
//        yield return new WaitForSeconds(7.3f);
//        oilStoune = true;
//    }

//}
using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class RockThrow : NetworkBehaviour
{
    [Header("Stones")]
    [SerializeField] private GameObject rockPrefab; // Префаб камня
    [SerializeField] private bool rockStoune = true; // Доступность камня для броска
    [SerializeField] private GameObject oilrockPrefab; // Префаб замедляющего камня
    [SerializeField] private bool oilStoune = true; // Доступность замедляющего камня
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

        if (Input.GetKeyDown(KeyCode.K) && rockStoune) // Бросок обычного камня
        {
            rockStoune = false;
            numberAttack = 1;
            StartCoroutine(PlayerAttackRockStoune());
        }
        else if (Input.GetKeyDown(KeyCode.J) && oilStoune) // Бросок замедляющего камня
        {
            oilStoune = false;
            numberAttack = 2;
            StartCoroutine(PlayerAttackOilStoune());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ThrowRockServerRpc()
    {
        // Проверка корректности атаки
        if (numberAttack != 1 && numberAttack != 2)
        {
            Debug.LogError("Invalid attack number: " + numberAttack);
            return;
        }

        // Создаём объект на сервере
        Transform rockInstance;
        if (numberAttack == 1)
        {
            rockInstance = Instantiate(rockPrefab.transform, throwPosition.position, Quaternion.identity);
        }
        else if (numberAttack == 2)
        {
            rockInstance = Instantiate(oilrockPrefab.transform, throwPosition.position, Quaternion.identity);
        }
        else
        {
            Debug.LogError("Unexpected attack number: " + numberAttack);
            return;
        }

        // Спавним объект в сети
        NetworkObject networkObject = rockInstance.GetComponent<NetworkObject>();
        networkObject.Spawn();

        // Настраиваем физику камня
        SetupRockPhysics(rockInstance);

        // Уведомляем клиентов о создании объекта (если требуется)
        ThrowRockClientRpc(networkObject.NetworkObjectId);
    }

    [ClientRpc]
    public void ThrowRockClientRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            Transform rockInstance = networkObject.transform;
            SetupRockPhysics(rockInstance);
        }
    }

    private void SetupRockPhysics(Transform rockInstance)
    {
        Rigidbody2D rb = rockInstance.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float throwAngleRad = throwAngle * Mathf.Deg2Rad;
            Vector2 throwDirection = new Vector2(tr.localScale.x * Mathf.Cos(throwAngleRad), Mathf.Sin(throwAngleRad));
            rb.velocity = throwDirection * throwForce;
        }
    }

    private IEnumerator PlayerAttackRockStoune()
    {
        AttackAnimation.SetBool("MoveRun", false);
        AttackAnimation.SetBool("MoveJump", false);
        AttackAnimation.SetBool("Attack", true);

        // Установить тип атаки
        numberAttack = 1;

        yield return new WaitForSeconds(0.2f); // Задержка перед броском
        ThrowRockServerRpc(); // Вызываем бросок на сервере

        yield return new WaitForSeconds(0.5f);
        AttackAnimation.SetBool("Attack", false);

        yield return new WaitForSeconds(4.3f); // Задержка перед доступностью следующего броска
        rockStoune = true;
    }

    private IEnumerator PlayerAttackOilStoune()
    {
        AttackAnimation.SetBool("MoveRun", false);
        AttackAnimation.SetBool("MoveJump", false);
        AttackAnimation.SetBool("Attack", true);

        // Установить тип атаки
        numberAttack = 2;

        yield return new WaitForSeconds(0.2f); // Задержка перед броском
        ThrowRockServerRpc(); // Вызываем бросок на сервере

        yield return new WaitForSeconds(0.5f);
        AttackAnimation.SetBool("Attack", false);

        yield return new WaitForSeconds(7.3f); // Задержка перед доступностью следующего броска
        oilStoune = true;
    }
}

