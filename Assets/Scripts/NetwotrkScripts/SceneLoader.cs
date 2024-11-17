using System.Diagnostics;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public static class SceneLoader
{

    public enum Scene
    {
        MenuGame,
        LobbyGame,
        GameScene,
        LoadingScene,
        ConnectionMenu
    }

    private static Scene _targetScene;

    public static void Load(Scene targetScene)
    {
        SceneLoader._targetScene = targetScene;

        SceneManager.LoadScene(Scene.LoadingScene.ToString());
    }

    public static Scene GetTargetScene()
    {
        return _targetScene;
    }

    public static void LoadNetwork(Scene targetScene)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(targetScene.ToString(), LoadSceneMode.Single);
    }

    public static void LoaderCallback()
    {
        SceneManager.LoadScene(_targetScene.ToString());
    }

}