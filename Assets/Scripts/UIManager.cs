using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI matchesText;
    public TextMeshProUGUI turnText;

    [SerializeField] TextMeshProUGUI lastMatchText;
    [SerializeField] TextMeshProUGUI lastTurnText;
    [SerializeField] GameObject gameOverScreen;

    public static UIManager instance;

    void Start()
    {
        instance = this;

        Init();
    }

    private void Init()
    {
        lastMatchText.text = $"Matches: {PlayerPrefs.GetInt("SaveMatches")}";
        lastTurnText.text = $"Turn: {PlayerPrefs.GetInt("SaveTurns")}";
    }

    public void UpdateMatchesUI(int matchesCount)
    {
        if (matchesText != null)
        {
            matchesText.text = $"Matches: {matchesCount}";
            PlayerPrefs.SetInt("SaveMatches", matchesCount);
        }
    }

    public void UpdateTurnsUI(int turnCount)
    {
        if (turnText != null)
        {
            turnText.text = $"Turns: {turnCount}";
            PlayerPrefs.SetInt("SaveTurns", turnCount);
        }
    }

    public void OnClickReloadBtn()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void ShowGameOverScreen()
    {
        gameOverScreen.SetActive(true);
    }
}
