using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using WebSocketSharp;

public class ConnectionMenu : MonoBehaviour
{
    public TMP_InputField nameInputField;

    //public void OnConnectButtonClicked()
    //{
    //    string playerName = nameInputField.text;
    //    if (!string.IsNullOrEmpty(playerName))
    //    {
    //        PlayerData.Instance.SetPlayerName(playerName); // Сохранение имени
    //        SceneManager.LoadScene("MenuGame"); // Переход на следующую сцену
    //    }
    //    else
    //    {
    //        Debug.Log("Имя не может быть пустым.");
    //    }
    //}
    public async void OnConnectButtonClicked()
    {
        if (UnityServices.State != ServicesInitializationState.Initialized)
        {
            if (!string.IsNullOrEmpty(nameInputField.text))
            {
                //PlayerData.Instance.SetPlayerName(nameInputField.text); // Сохранение имени
                SceneManager.LoadScene("MenuGame"); // Переход на следующую сцену
                InitializationOptions initializationOptions = new();
                initializationOptions.SetProfile(nameInputField.text);

                await UnityServices.InitializeAsync(initializationOptions);

                AuthenticationService.Instance.SignedIn += () =>
                {
                    Debug.Log($"{gameObject.name} Signed in! " + AuthenticationService.Instance.PlayerId);
                    Debug.Log($"{gameObject.name} Player name: " + nameInputField.text);
                };

                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            else
            {
                Debug.Log("Имя не может быть пустым.");
            }
        }
    }
}
