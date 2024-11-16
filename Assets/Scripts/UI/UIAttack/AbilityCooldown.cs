using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityCooldown : MonoBehaviour
{
    public Image cooldownImage; // UI-�������, ������� ����� �����������
    public TMP_Text cooldownText;  // UI-����� ��� ����������� �������
    public float cooldownTime = 5f; // ������������ ��������
    public KeyCode abilityKey = KeyCode.K; // ������ ��� ��������� �����������

    private float cooldownTimer; // ������� ����� ��������
    private bool isOnCooldown; // ���� ��������

    private void Start()
    {
        // ��������, ��� ����������� � ����� ��������� � ������
        if (cooldownImage != null)
            cooldownImage.gameObject.SetActive(false);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // ���� ������ ������� � ����������� �� �� ��������
        if (Input.GetKeyDown(abilityKey) && !isOnCooldown)
        {
            ActivateAbility();
        }

        // ��������� ������������ ��������, ���� �� �������
        if (isOnCooldown)
        {
            UpdateCooldownUI();
        }
    }

    private void ActivateAbility()
    {
        // ��������� �������
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        // �������� ����������� � �����
        if (cooldownImage != null)
            cooldownImage.gameObject.SetActive(true);

        if (cooldownText != null)
            cooldownText.gameObject.SetActive(true);

        // ����� ����� �������� ������ ��������� �����������
        Debug.Log("Ability activated!");
    }

    private void UpdateCooldownUI()
    {
        // ��������� ������
        cooldownTimer -= Time.deltaTime;

        // ������������ �������� ���������� (�� 1 �� 0)
        float fillAmount = cooldownTimer / cooldownTime;
        if (cooldownImage != null)
            cooldownImage.fillAmount = fillAmount;

        // ��������� ����� ��������
        if (cooldownText != null)
        {
            int secondsLeft = Mathf.CeilToInt(cooldownTimer); // ���������� �������
            cooldownText.text = secondsLeft.ToString();
        }

        // ���� ������� ��������
        if (cooldownTimer <= 0)
        {
            isOnCooldown = false;

            // ��������� ����������� � �����
            if (cooldownImage != null)
                cooldownImage.gameObject.SetActive(false);

            if (cooldownText != null)
                cooldownText.gameObject.SetActive(false);

            Debug.Log("Ability ready!");
        }
    }
}
