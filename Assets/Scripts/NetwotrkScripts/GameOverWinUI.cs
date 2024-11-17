using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOverWinUI : MonoBehaviour
{
    public static GameOverWinUI Instance;

    public UnityEvent PlayerExit = new();

    [SerializeField] private TextMeshProUGUI _gameOverWinText;
    [SerializeField] private Button _playAgainButton;


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
        GameStateManager.Instance.OnStateChanged.AddListener(GameManager_OnStateChanged);
        GameStateManager.Instance.OnOpenHUD.AddListener(() => { Show(); });
        GameStateManager.Instance.OnCloseHUD.AddListener(() => { Hide(); });
        _playAgainButton.gameObject.SetActive(false);
        Hide();
    }

    private void GameManager_OnStateChanged()
    {
        if (GameStateManager.Instance.GetGameState().ToString() == "GameOver")
        {
            ulong clientID = MultiplayerStorage.Instance.GetPlayerData().clientId;
            GameStateManager.Instance.ReportPlayerLostServerRpc(clientID);
            if (NetworkManager.Singleton.LocalClientId == clientID)
                _gameOverWinText.text = $"{MultiplayerStorage.Instance.GetPlayerName()} you champion!!";
            else
                _gameOverWinText.text = $"You lose((((";
            _playAgainButton.gameObject.SetActive(true);
            RemoveListeners();
            Show();
        }
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
