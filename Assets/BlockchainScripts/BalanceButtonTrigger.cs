using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalanceButtonTrigger : MonoBehaviour
{
    [Header("Button để kích hoạt FetchBalances")]
    public Button fetchBalanceButton;
    public TextMeshProUGUI revivalTokenText;
    private const string REVIVAL_PREF_KEY = "RevivalTokenCount";

    private void Start()
    {
        if (fetchBalanceButton != null)
        {
            fetchBalanceButton.onClick.AddListener(OnFetchBalanceClicked);
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán button trong BalanceButtonTrigger.");
        }
    }

    public void OnFetchBalanceClicked()
    {
        // Gọi hàm FetchBalances từ singleton
        if (BalanceFetcher.Instance != null)
        {
            BalanceFetcher.Instance.FetchBalances();
            FindObjectOfType<TokenDisplayUI>()?.UpdateTokenDisplay();
            FindObjectOfType<WalletBalanceDisplay>()?.UpdateWalletBalance();
            int current = PlayerPrefs.GetInt(REVIVAL_PREF_KEY, 0);
            if (revivalTokenText != null)
                revivalTokenText.text = $"x {current}";
            FindObjectOfType<VIPNFTDisplay>()?.UpdateVIPNFTDisplay();
        }
        else
        {
            Debug.LogError("❌ BalanceFetcher.Instance chưa khởi tạo.");
        }
    }
}
