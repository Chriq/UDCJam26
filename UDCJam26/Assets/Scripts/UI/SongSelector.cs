using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public class HighScoreElement
{
    public TextMeshProUGUI textUI;
    public string sceneName;
}

public class SongSelector : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvas;

    [SerializeField] HighScoreElement[] elementsHighScore;

    void Start()
    {
        foreach (HighScoreElement e in elementsHighScore)
        {
            if (PlayerPrefs.GetInt($"HIGH_SCORE_{e.sceneName}_UPDATED", 0) == 1)
            {
                e.textUI.text = $"*** High Score: {PlayerPrefs.GetFloat($"HIGH_SCORE_{e.sceneName}", 0).ToString("N2")}";
                PlayerPrefs.SetInt($"HIGH_SCORE_{e.sceneName}_UPDATED", 0);
            }
            else
            {
                e.textUI.text = $"High Score: {PlayerPrefs.GetFloat($"HIGH_SCORE_{e.sceneName}", 0).ToString("N2")}";
            }
        }
    }

    public void StartSong(string sceneName)
    {
        UIManager.Instance.fade.canvas = fadeCanvas;
        UIManager.Instance.fade.FadeInWithCallback(delegate
        {
            SceneManager.LoadScene(sceneName);
        }, 1f);
    }

    public void ToMainMenu()
    {
        UIManager.Instance.fade.canvas = fadeCanvas;
        UIManager.Instance.fade.FadeInWithCallback(delegate
        {
            SceneManager.LoadScene("MainMenu");
        }, 3f);
    }
}
