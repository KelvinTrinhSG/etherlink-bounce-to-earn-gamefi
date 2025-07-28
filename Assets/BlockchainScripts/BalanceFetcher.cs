using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Thirdweb;
using UnityEngine;

public class BalanceFetcher : MonoBehaviour
{
    // 🔸 ADDED: Singleton Instance
    public static BalanceFetcher Instance { get; private set; } // 🔸 ADDED

    private string userAddress;

    // 🔸 ADDED: Setup Singleton
    private void Awake() // 🔸 ADDED
    {
        if (Instance != null && Instance != this) // 🔸 ADDED
        {
            Destroy(gameObject); // 🔸 ADDED
            return; // 🔸 ADDED
        } // 🔸 ADDED

        Instance = this; // 🔸 ADDED
        DontDestroyOnLoad(gameObject); // Optional if you want it to persist between scenes // 🔸 ADDED
    }

    public async void FetchBalances()
    {
        try
        {
            // 🔹 Lấy địa chỉ ví người dùng
            userAddress = await ThirdwebManager.Instance.SDK.Wallet.GetAddress();

            // 🔹 Lấy số dư ví (native token, ví dụ ETH, MATIC...)
            var bal = await ThirdwebManager.Instance.SDK.Wallet.GetBalance();
            PlayerDataManager.Instance.walletBalance = bal.displayValue; // 🟢 Thêm dòng này

            // 🔹 Lấy số lượng NFT VIP sở hữu
            var nftContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.NFTVIPContractAddress);
            List<NFT> nftList = await nftContract.ERC721.GetOwned(userAddress);
            PlayerDataManager.Instance.vipNFT = nftList.Count;

            // 🔹 Lấy số GEM token sở hữu
            var tokenContract = ThirdwebManager.Instance.SDK.GetContract(GameConfig.TokenGemContractAddress);
            var balance = await tokenContract.ERC20.BalanceOf(userAddress);

            // 🔹 Chuyển displayValue (string) -> int
            if (int.TryParse(balance.displayValue.Split('.')[0], NumberStyles.Any, CultureInfo.InvariantCulture, out int parsedToken))
            {
                PlayerDataManager.Instance.gemToken = parsedToken;
            }
            else
            {
                Debug.LogWarning("⚠️ Failed to parse GEM token balance.");
                PlayerDataManager.Instance.gemToken = 0;
            }

            Debug.Log($"✅ NFT VIP: {PlayerDataManager.Instance.vipNFT}, GEM Token: {PlayerDataManager.Instance.gemToken}");
            PlayerDataManager.Instance.isChainInitialized = true;
        }
        catch (Exception ex)
        {
            Debug.LogError("❌ Error fetching balances: " + ex.Message);
        }
    }
}
