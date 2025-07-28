using TMPro;
using UnityEngine;
using UnityEngine.UI; // 🟦 để dùng Button

public class VIPNFTDisplay : MonoBehaviour
{
    [Header("Text hiển thị số lượng VIP NFT")]
    public TextMeshProUGUI vipNFTText;

    [Header("Nút mua VIP NFT sẽ bị ẩn nếu đã sở hữu")]
    public GameObject buyVIPButton; // 🟦 có thể là Button hoặc GameObject tùy thiết kế

    private void Start()
    {
        UpdateVIPNFTDisplay();
    }

    /// <summary>
    /// Gọi hàm này để cập nhật lại text số lượng VIP NFT.
    /// </summary>
    public void UpdateVIPNFTDisplay()
    {
        if (vipNFTText != null)
        {
            int vipCount = PlayerDataManager.Instance.vipNFT;
            vipNFTText.text = $"x {vipCount}";

            // 🟦 Nếu người chơi đã có ít nhất 1 VIP NFT, ẩn nút mua
            if (buyVIPButton != null)
            {
                buyVIPButton.SetActive(vipCount <= 0);
            }
        }
        else
        {
            Debug.LogWarning("❗ Chưa gán TextMeshProUGUI cho VIPNFTDisplay.");
        }
    }
}
