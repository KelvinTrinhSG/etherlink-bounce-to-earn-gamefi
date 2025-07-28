using TMPro;
using UnityEngine;

public class TokenDisplayUI : MonoBehaviour
{
    [Header("Text hiển thị số lượng GEM Token")]
    public TextMeshProUGUI gemTokenText;

    private void Start()
    {
        UpdateTokenDisplay();
    }

    /// <summary>
    /// Gọi hàm này để cập nhật lại text số lượng GEM token.
    /// </summary>
    public void UpdateTokenDisplay()
    {
        if (gemTokenText != null)
        {
            int gemCount = PlayerDataManager.Instance.gemToken;
            gemTokenText.text = $"x {gemCount}";
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán TextMeshProUGUI cho TokenDisplay.");
        }
    }
}
