using UnityEngine;

public class GameOverHandler : MonoBehaviour
{
    public PlayerScoreSender scoreSender;

    public void OnGameOver(int score)
    {
        string playerEmail = PlayerPrefs.GetString("player_email", "default@email.com");
        string playerWallet = PlayerPrefs.GetString("player_wallet", "default_wallet");

        scoreSender.SendScore(playerEmail, playerWallet, score);
    }
}