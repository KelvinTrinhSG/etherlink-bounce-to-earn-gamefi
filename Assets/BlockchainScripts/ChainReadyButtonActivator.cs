using UnityEngine;
using UnityEngine.UI;

public class ChainReadyButtonActivator : MonoBehaviour
{
    [Header("Buttons to Enable")]
    public Button web3ShopButton;
    public Button playGameButton;
    public Button leaderboardButton;

    private bool buttonsActivated = false;

    void Update()
    {
        if (!buttonsActivated && PlayerDataManager.Instance != null && PlayerDataManager.Instance.isChainInitialized)
        {
            ActivateButtons();
            buttonsActivated = true; // tránh kích hoạt nhiều lần
        }
    }

    private void ActivateButtons()
    {
        if (web3ShopButton != null) web3ShopButton.interactable = true;
        if (playGameButton != null) playGameButton.interactable = true;
        if (leaderboardButton != null) leaderboardButton.interactable = true;

        Debug.Log("✅ Blockchain initialized. Buttons activated.");
    }
}
