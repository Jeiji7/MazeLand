using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    public GameObject consoleUI; // UI �������
    public TMP_InputField inputField; // ���� ����� �������
    public ScrollRect scrollRect; // ScrollRect ��� ���������
    public GameObject textPrefab; // ������ ��� ��������� ��������
    public Transform contentTransform; // ��������� ��� ������� (Content ������ ScrollRect)

    private bool isConsoleVisible = false;

    private void Update()
    {
        // �������/������� ������� �� ������� �� "`"
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }

        // ���� ������� ������� � ������ ������� Enter
        if (isConsoleVisible && Input.GetKeyDown(KeyCode.Return))
        {
            string command = inputField.text;
            ExecuteCommand(command);
            inputField.text = ""; // ������� ���� �����
            inputField.ActivateInputField(); // ����� ������������ ���� �����
        }
    }

    private void ToggleConsole()
    {
        isConsoleVisible = !isConsoleVisible;
        consoleUI.SetActive(isConsoleVisible);

        if (isConsoleVisible)
        {
            inputField.ActivateInputField(); // ���������� ���� ��� ����� ������
        }
        else
        {
            // ��� �������� ������� ������� ��� ������
            ClearConsole();
        }
    }

    private void ClearConsole()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject); // ������� ������ ��������� ��������� ������
        }
    }

    private void ExecuteCommand(string command)
    {
        // ������ ���������� �������
        LogMessage($"����������� �������: {command}");

        // ���������� ������� ����� CommandManager (���� ���������)
        FindObjectOfType<CommandManager>().ExecuteCommand(command);
    }

    public void LogMessage(string message)
    {
        // ������� ����� ��������� ������
        GameObject newTextObject = Instantiate(textPrefab, contentTransform);

        // ����������� �����
        TMP_Text newText = newTextObject.GetComponent<TMP_Text>();
        newText.text = message;

        // ������������ ScrollRect ����
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // ������������ � ���������� ���������
    }
}
