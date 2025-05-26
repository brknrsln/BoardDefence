using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text levelText;
    [SerializeField] private Button startButton;

    private void Awake()
    {
        if (!levelText)
        {
            Debug.LogError("Level Text is not assigned in MainMenuController.");
        }

        if (!startButton)
        {
            Debug.LogError("Start Button is not assigned in MainMenuController.");
        }
        
        var currentLevel = PlayerData.Instance.Level;
        
        levelText.text = "Level: " + currentLevel;

        if (currentLevel >= Constants.LevelDataDictionary.Count)
        {
            startButton.interactable = false;
            startButton.GetComponentInChildren<TMPro.TMP_Text>().text = "Max Level Reached";
        }
        else
        {
            startButton.onClick.AddListener(StartGame);
        }
    }

    private void StartGame()
    {
        GameSceneManager.LoadLevelScene();
    }
}