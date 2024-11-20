using Mirror.BouncyCastle.Crypto.Utilities;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class GameOverWinUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _player;
    public static GameOverWinUI Instance;

    public UnityEvent PlayerExit = new();

    [SerializeField] private TextMeshProUGUI _gameOverWinText;
    [SerializeField] private Button _playAgainButton;

    public enum WinState
    {
        FirstWin,
        SecondWin,
        ThirdWin,
        FourthWin
    }
    public void SetPlayerIndex(int playerIndex)
    {
        // он даже не хочет картинку по индексу выдовать
        Show();
        
        _player[playerIndex].SetActive(true);
        _gameOverWinText.text = $"{MultiplayerStorage.Instance.GetPlayerDataFromPlayerIndex(playerIndex).playerName} you champion!!";
    }

    private void Awake()
    {
        Instance = this;
        _playAgainButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
            SceneLoader.Load(SceneLoader.Scene.ConnectionMenu);
            PlayerExit.Invoke();
        });
    }

    private void Start()
    {
        GameStateManager.Instance.OnOpenHUD.AddListener(() => { Show(); });
        GameStateManager.Instance.OnCloseHUD.AddListener(() => { Hide(); });
        //_playAgainButton.gameObject.SetActive(false);
        Hide();
    }



    private void RemoveListeners()
    {
        GameStateManager.Instance.OnCloseHUD.RemoveAllListeners();
        GameStateManager.Instance.OnOpenHUD.RemoveAllListeners();
    }

    private void Show()
    {
        gameObject.SetActive(true);
        Debug.Log($"{gameObject.name} SHOW");
        _playAgainButton.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
        Debug.Log($"{gameObject.name} HIDE");
    }
}



