using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    public Image cooldownImage; // UI-элемент, который будет заполняться
    public TMP_Text cooldownText;  // UI-текст для отображения времени
    public float cooldownTime = 5f; // Длительность кулдауна
    public KeyCode abilityKey = KeyCode.K; // Кнопка для активации способности

    private float cooldownTimer; // Текущее время кулдауна
    private bool isOnCooldown; // Флаг кулдауна

    private void Start()
    {
        // Убедимся, что изображение и текст выключены в начале
        if (cooldownImage != null)
            cooldownImage.gameObject.SetActive(false);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Если нажата клавиша и способность не на кулдауне
        if (Input.GetKeyDown(abilityKey) && !isOnCooldown)
        {
            ActivateAbility();
        }

        // Обновляем визуализацию кулдауна, если он активен
        if (isOnCooldown)
        {
            UpdateCooldownUI();
        }
    }

    private void ActivateAbility()
    {
        // Запускаем кулдаун
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        // Включаем изображение и текст
        if (cooldownImage != null)
            cooldownImage.gameObject.SetActive(true);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(true);

        // Здесь можно добавить логику активации способности
        Debug.Log("Ability activated!");
    }

    private void UpdateCooldownUI()
    {
        // Уменьшаем таймер
        cooldownTimer -= Time.deltaTime;

        // Рассчитываем прогресс заполнения (от 1 до 0)
        float fillAmount = cooldownTimer / cooldownTime;
        if (cooldownImage != null)
            cooldownImage.fillAmount = fillAmount;

        // Обновляем текст кулдауна
        if (cooldownText != null)
        {
            int secondsLeft = Mathf.CeilToInt(cooldownTimer); // Оставшиеся секунды
            cooldownText.text = secondsLeft.ToString();
        }

        // Если кулдаун завершён
        if (cooldownTimer <= 0)
        {
            isOnCooldown = false;

            // Выключаем изображение и текст
            if (cooldownImage != null)
                cooldownImage.gameObject.SetActive(false);

            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);

            Debug.Log("Ability ready!");
        }
    }
}
