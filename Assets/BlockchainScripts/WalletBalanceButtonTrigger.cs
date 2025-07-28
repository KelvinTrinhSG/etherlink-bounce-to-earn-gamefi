using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Thirdweb;
using System.Globalization;

public class WalletBalanceButtonTrigger : MonoBehaviour
{
    [Header("Button để cập nhật ví")]
    public Button refreshWalletButton;

    [Header("Text hiển thị số dư ví (XTZ)")]
    public TextMeshProUGUI walletBalanceText;

    private void Start()
    {
        if (refreshWalletButton != null)
        {
            refreshWalletButton.onClick.AddListener(UpdateWalletBalance);
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán nút refreshWalletButton.");
        }
    }

    public async void UpdateWalletBalance()
    {
        if (walletBalanceText == null)
        {
            Debug.LogWarning("❗ Chưa gán Text để hiển thị ví.");
            return;
        }

        try
        {
            // 🔹 Lấy địa chỉ ví người dùng
            string userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
            if (string.IsNullOrEmpty(userAddress))
            {
                Debug.LogWarning("❌ Wallet address rỗng.");
                return;
            }

            // 🔹 Lấy số dư từ ví
            var balance = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();

            // 🔹 Lưu vào singleton nếu cần
            PlayerDataManager.Instance.walletBalance = balance.displayValue; // 🟢 Thêm dòng này

            // 🔹 Cập nhật UI
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
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ Lỗi khi lấy số dư ví: {ex.Message}");
            walletBalanceText.text = "--";
        }
    }

}
