using System;
using System.Collections;
using System.Globalization;
using System.Threading.Tasks;
using Thirdweb;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BurnAndSpinManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Button[] allButtons; // 🟢 Thêm mảng tất cả nút để disable/enable
    public Button spinButton;
    public TextMeshProUGUI[] statusTexts;
    public TextMeshProUGUI revivalTokenText;

    private string walletAddress;
    private const string REVIVAL_PREF_KEY = "RevivalTokenCount";

    private void Start()
    {
        UpdateRevivalTokenUI();
    }

    public async void OnSpinClicked()
    {
        SetButtonsInteractable(false);
        SetStatusTexts("Checking token balance...");

        try
        {
            walletAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();
            if (string.IsNullOrEmpty(walletAddress))
                throw new Exception("Wallet address is empty.");

            var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
            var balance = await tokenContract.ERC20.BalanceOf(walletAddress);

            int gemToken;
            if (!int.TryParse(balance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out gemToken))
                gemToken = 0;

            if (gemToken < 1)
            {
                SetStatusTexts("Not enough GEM. Please claim more.");
                SetButtonsInteractable(true); // mở lại nút nếu không đủ
                return;
            }

            SetStatusTexts("Burning 1 GEM...");
            await tokenContract.ERC20.Burn("1");

            SetStatusTexts("Spinning...");
            //await Task.Delay(300); // Chờ chút cho cảm giác mượt

            // ⬇️ Tách SpinEffect ra và đợi hoàn tất rồi mới mở lại nút
            StartCoroutine(SpinThenEnableButtons());
        }
        catch (Exception ex)
        {
            Debug.LogError("Spin failed: " + ex.Message);
            SetStatusTexts("Something went wrong. Try again.");
            SetButtonsInteractable(true);
        }
    }


    private void SetButtonsInteractable(bool state)
    {
        foreach (var btn in allButtons)
        {
            if (btn != null) btn.interactable = state;
        }
    }

    private void SetStatusTexts(string message)
    {
        foreach (var txt in statusTexts)
        {
            if (txt != null) txt.text = message;
        }
    }

    private void AddRevivalToken(int amount)
    {
        int current = PlayerPrefs.GetInt(REVIVAL_PREF_KEY, 0);
        current += amount;
        PlayerPrefs.SetInt(REVIVAL_PREF_KEY, current);
        PlayerPrefs.Save();
    }

    private void UpdateRevivalTokenUI()
    {
        int current = PlayerPrefs.GetInt(REVIVAL_PREF_KEY, 0);
        if (revivalTokenText != null)
            revivalTokenText.text = $"x {current}";
    }

    private IEnumerator SpinThenEnableButtons()
    {
        int finalReward = UnityEngine.Random.Range(1, 11);
        float spinDuration = 1.5f;  // thời gian quay tổng
        float interval = 0.1f;      // mỗi lần nhảy số
        float elapsed = 0f;

        while (elapsed < spinDuration)
        {
            int fakeReward = UnityEngine.Random.Range(1, 11);
            SetStatusTexts($"Spinning... {fakeReward}");

            yield return new WaitForSeconds(interval);
            elapsed += interval;
        }

        AddRevivalToken(finalReward);
        UpdateRevivalTokenUI();
        SetStatusTexts($"You won {finalReward} revival tokens!");
        // ✅ Quan trọng: bật lại tất cả các button
        SetButtonsInteractable(true);
        UpdateTokenUI();
    }

    private async void UpdateTokenUI()
    {
        var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
        // 🟢 Cập nhật số dư token
        var balance = await tokenContract.ERC20.BalanceOf(walletAddress);
        if (int.TryParse(balance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedToken))
        {
            PlayerDataManager.Instance.gemToken = parsedToken;
        }
        else
        {
            Debug.LogWarning("⚠️ Failed to parse GEM token balance.");
            PlayerDataManager.Instance.gemToken = 0;
        }

        // 🔹 Lấy địa chỉ ví người dùng
        string userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

        // 🔹 Lấy số dư ví (native token, ví dụ ETH, MATIC...)
        var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
        PlayerDataManager.Instance.walletBalance = bal.displayValue; // 🟢 Thêm dòng này

        // 🟢 UI sau khi thành công
        SetStatusTexts("Completed.");
        FindObjectOfType<TokenDisplayUI>()?.UpdateTokenDisplay();
        FindObjectOfType<WalletBalanceDisplay>()?.UpdateWalletBalance();
    }
}
