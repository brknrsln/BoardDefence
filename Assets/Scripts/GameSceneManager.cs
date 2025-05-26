using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameSceneManager
{
    private const string MainMenuScene = "Main Menu";
    private const string LevelScene = "Level Scene";

    public static void ReloadActiveScene()
    {
        var activeSceneName = SceneManager.GetActiveScene().name;

        DOTween.KillAll();

        SceneManager.LoadSceneAsync(activeSceneName);
        
        Debug.Log("Reloading active scene: " + activeSceneName);
    }

    private static void LoadScene(string sceneName)
    {
        if (sceneName == SceneManager.GetActiveScene().name)
        {
            Debug.LogWarning("Already in scene: " + sceneName);
            return;
        }

        DOTween.KillAll();

        SceneManager.LoadSceneAsync(sceneName);

        Debug.Log("Loading scene: " + sceneName);
    }
    
    public static void LoadMainMenu()
    {
        LoadScene(MainMenuScene);
    }
    
    public static void LoadLevelScene()
    {
        LoadScene(LevelScene);
    }
}