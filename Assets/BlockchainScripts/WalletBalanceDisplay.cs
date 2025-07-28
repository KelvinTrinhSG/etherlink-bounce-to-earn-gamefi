using System.Globalization;
using TMPro;
using UnityEngine;

public class WalletBalanceDisplay : MonoBehaviour
{
    [Header("Text hiển thị số dư ví")]
    public TextMeshProUGUI walletBalanceText;

    private void Start()
    {
        UpdateWalletBalance();
    }

    /// <summary>
    /// Gọi hàm này để cập nhật lại text hiển thị số dư ví.
    /// </summary>
    public void UpdateWalletBalance()
    {
        if (walletBalanceText != null)
        {
            string balanceStr = PlayerDataManager.Instance.walletBalance;

            if (decimal.TryParse(balanceStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal balanceDecimal))
            {
                // Hiển thị với 2 chữ số sau dấu chấm
                walletBalanceText.text = balanceDecimal.ToString("F2", CultureInfo.InvariantCulture);
            }
            else
            {
                walletBalanceText.text = "0.00";
            }
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán TextMeshProUGUI cho WalletBalanceDisplay.");
        }
    }
}
