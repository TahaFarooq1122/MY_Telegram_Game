using UnityEngine;

public class TelegramBridge : MonoBehaviour
{

    public static TelegramBridge Instance { get; private set; }

    void Awake()
    {
        // Ensure a single instance
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Persist between scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // 🚀 Post score to Telegram Web App leaderboard
    public static void PostScore(int score)
    {
        Debug.Log("📤 Posting score to Telegram: " + score);
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("telegram_post_score", score);
#endif
    }

    // 📨 You can call this to request user data from JS
    public static void RequestTelegramUserData()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        Application.ExternalCall("telegram_get_user_data");
#endif
    }
}