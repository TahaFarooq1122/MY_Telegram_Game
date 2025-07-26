using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class PlayerScoreSender : MonoBehaviour
{
    private const string apiUrl = "https://2fb12c3a-6c60-4c66-be64-ce43644a31fe-00-108mzd0osjh6a.sisko.replit.dev/api/players/save";

    public void SendScore(string email, string wallet, int score)
    {
        Debug.Log($"📤 Sending score: {score}, Email: {email}, Wallet: {wallet}");
        StartCoroutine(SendScoreCoroutine(email, wallet, score));
    }

    private IEnumerator SendScoreCoroutine(string email, string wallet, int score)
    {
        var scoreData = new ScoreData
        {
            email = email,
            wallet = wallet,
            score = score
        };

        string jsonData = JsonUtility.ToJson(scoreData);
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Score sent successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Failed to send score: " + request.error);
                Debug.LogError("❗ Response: " + request.downloadHandler.text);
            }
        }
    }

    [System.Serializable]
    public class ScoreData
    {
        public string email;
        public string wallet;
        public int score;
    }
}