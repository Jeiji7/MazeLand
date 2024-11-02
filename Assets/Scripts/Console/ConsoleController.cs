using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleController : MonoBehaviour
{
    public GameObject consoleUI; // UI консоли
    public TMP_InputField inputField; // Поле ввода команды
    public ScrollRect scrollRect; // ScrollRect для прокрутки
    public GameObject textPrefab; // Префаб для текстовых объектов
    public Transform contentTransform; // Контейнер для текстов (Content внутри ScrollRect)

    private bool isConsoleVisible = false;

    private void Update()
    {
        // Открыть/закрыть консоль по нажатию на "`"
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            ToggleConsole();
        }

        // Если консоль активна и нажата клавиша Enter
        if (isConsoleVisible && Input.GetKeyDown(KeyCode.Return))
        {
            string command = inputField.text;
            ExecuteCommand(command);
            inputField.text = ""; // Очистка поля ввода
            inputField.ActivateInputField(); // Снова активировать поле ввода
        }
    }

    private void ToggleConsole()
    {
        isConsoleVisible = !isConsoleVisible;
        consoleUI.SetActive(isConsoleVisible);

        if (isConsoleVisible)
        {
            inputField.ActivateInputField(); // Активируем поле для ввода текста
        }
        else
        {
            // При закрытии консоли удаляем все записи
            ClearConsole();
        }
    }

    private void ClearConsole()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject); // Удаляем каждый созданный текстовый объект
        }
    }

    private void ExecuteCommand(string command)
    {
        // Логика выполнения команды
        LogMessage($"Выполняется команда: {command}");

        // Выполнение команды через CommandManager (если требуется)
        FindObjectOfType<CommandManager>().ExecuteCommand(command);
    }

    public void LogMessage(string message)
    {
        // Создаем новый текстовый объект
        GameObject newTextObject = Instantiate(textPrefab, contentTransform);

        // Настраиваем текст
        TMP_Text newText = newTextObject.GetComponent<TMP_Text>();
        newText.text = message;

        // Прокручиваем ScrollRect вниз
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f; // Прокручиваем к последнему сообщению
    }
}
