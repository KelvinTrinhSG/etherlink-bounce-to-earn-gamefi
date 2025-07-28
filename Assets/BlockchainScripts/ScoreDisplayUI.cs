using TMPro;
using UnityEngine;

public class ScoreDisplayUI : MonoBehaviour
{
    [Header("UI hiển thị tổng điểm")]
    public TextMeshProUGUI scoreText;

    private const string PREF_KEY = "TotalScore";

    public void UpdateTotalScoreInShop()
    {
        int current = PlayerPrefs.GetInt(PREF_KEY, 0);

        if (scoreText != null)
        {
            scoreText.text = current.ToString();
        }
        else
        {
            Debug.LogWarning("❗ scoreText chưa được gán.");
        }
    }
}
