using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class TONManager : MonoBehaviour
{
    // WebGL ↔ JS bridge
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")] private static extern void ConnectWalletJS();
    [DllImport("__Internal")] private static extern string GetWalletAddressJS();
    [DllImport("__Internal")] private static extern void SendRewardJS(string address);
    [DllImport("__Internal")] private static extern void PostScoreJS(int score);
#endif

    [Header("UI References")]
    public InputField emailInputField;
    public Text feedbackText;

    [Header("Backend Configuration")]
    [Tooltip("Your live or local backend URL")]
    public string backendURL = "http://localhost:5000/api/players/save";

    private string walletAddress = "";
    private string userEmail = "";

    void Start()
    {
        RequestWalletAddress(); // Automatically fetch address if cached
    }

    public void ConnectWallet()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        ConnectWalletJS();
#else
        Debug.Log("🔧 ConnectWallet() called in Editor");
        OnWalletAddressReceived("EQ_fake_wallet_address_debug");
#endif
    }

    public void RequestWalletAddress()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        string address = GetWalletAddressJS();
        OnWalletAddressReceived(address);
#else
        Debug.Log("🔧 RequestWalletAddress() called in Editor");
        OnWalletAddressReceived("EQ_fake_wallet_address_debug");
#endif
    }

    public void SendReward(string address)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SendRewardJS(address);
#else
        Debug.Log("🔧 SendReward called to: " + address);
#endif
    }

    public void PostScore(int score)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        PostScoreJS(score);
#else
        Debug.Log("🔧 PostScore: " + score);
#endif
    }

    // ✅ Called from JS or internally
    public void OnWalletAddressReceived(string address)
    {
        walletAddress = address;
        Debug.Log("✅ Wallet Address Received: " + walletAddress);

        // Attempt validation only after email is input
        ValidateInputs();
    }

    private void ValidateInputs()
    {
        if (emailInputField == null || feedbackText == null)
        {
            Debug.LogWarning("❗ UI references not set on TONManager.");
            return;
        }

        userEmail = emailInputField.text.Trim();

        if (!IsValidEmail(userEmail))
        {
            feedbackText.text = "❌ Invalid email address.";
            return;
        }

        if (!IsValidWallet(walletAddress))
        {
            feedbackText.text = "❌ Invalid wallet address.";
            return;
        }

        feedbackText.text = "✅ Validating...";

        SaveToLocal();

        // 🔄 Sync to GameManager if it exists
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.playerEmail = userEmail;
            gm.playerWallet = walletAddress;
        }

        StartCoroutine(SendPlayerDataToBackend());
    }

    private bool IsValidEmail(string email)
    {
        return !string.IsNullOrEmpty(email) && email.Contains("@") && email.EndsWith(".com");
    }

    private bool IsValidWallet(string address)
    {
        return !string.IsNullOrEmpty(address) && address.StartsWith("EQ") && address.Length >= 48;
    }

    private void SaveToLocal()
    {
        PlayerPrefs.SetString("UserEmail", userEmail);
        PlayerPrefs.SetString("UserWallet", walletAddress);
        PlayerPrefs.Save();

        Debug.Log("✅ User data saved locally to PlayerPrefs.");
    }

    private IEnumerator SendPlayerDataToBackend()
    {
        if (string.IsNullOrEmpty(backendURL))
        {
            Debug.LogError("❌ Backend URL not set.");
            feedbackText.text = "❌ Backend URL missing.";
            yield break;
        }

        PlayerData player = new PlayerData
        {
            email = userEmail,
            wallet = walletAddress,
            score = 0 // Optional: send actual score later
        };

        string jsonData = JsonUtility.ToJson(player);

        using (UnityWebRequest request = UnityWebRequest.Put(backendURL, jsonData))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Player data successfully sent to backend.");
                feedbackText.text = "✅ Data sent successfully.";
            }
            else
            {
                Debug.LogError("❌ Error sending data: " + request.error);
                feedbackText.text = "❌ Failed to send data.";
            }
        }
    }

    [System.Serializable]
    public class PlayerData
    {
        public string email;
        public string wallet;
        public int score;
    }
}
