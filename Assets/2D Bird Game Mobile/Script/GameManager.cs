using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Text textOver;
    public Image imageOver;

    [Header("Player Data")]
    public int playerScore;
    public string playerEmail = "test@example.com"; // Will be set via JS
    public string playerWallet = "EQ1234567890abcdef"; // Will be set via JS

    [Header("Score Sender")]
    public PlayerScoreSender scoreSender;

    [Header("Score Handler")]
    public GameOverHandler gameOverHandler;

    void Start()
    {
        if (scoreSender == null)
            scoreSender = GetComponent<PlayerScoreSender>();

        if (gameOverHandler == null)
            gameOverHandler = GetComponent<GameOverHandler>();
    }

    public void ResGame()
    {
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }

    public void Play()
    {
        TONManager ton = FindObjectOfType<TONManager>();
        if (ton != null)
        {
            ton.ConnectWallet();
        }

        SceneManager.LoadScene("Game");
    }

    public void Menu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1;
    }

    public void GameOver()
    {
        // Show Game Over UI
        textOver.gameObject.SetActive(true);
        imageOver.gameObject.SetActive(true);
        Time.timeScale = 0;

        // Send score through GameOverHandler (handles backend)
        if (gameOverHandler != null)
        {
            gameOverHandler.OnGameOver(playerScore);
        }
        else if (scoreSender != null)
        {
            scoreSender.SendScore(playerEmail, playerWallet, playerScore);
        }
        else
        {
            Debug.LogWarning("❌ No GameOverHandler or ScoreSender assigned!");
        }

        // Post to Telegram
        TelegramBridge.PostScore(playerScore);
    }

    // Called from JS
    public void SetPlayerInfo(string json)
    {
        Debug.Log("Received player info JSON: " + json);
        PlayerInfo info = JsonUtility.FromJson<PlayerInfo>(json);
        playerEmail = info.email;
        playerWallet = info.wallet;
    }

    [System.Serializable]
    public class PlayerInfo
    {
        public string email;
        public string wallet;
    }
}