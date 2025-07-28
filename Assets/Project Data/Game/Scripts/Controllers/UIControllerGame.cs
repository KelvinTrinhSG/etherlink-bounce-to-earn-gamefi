using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class UIControllerGame : MonoBehaviour
{
    private static UIControllerGame instance;

    [Header("Settings")]
    public float reviveWaitTime;

    [Header("References")]
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject revivePanel;
    public GameObject navigationButtonsPanel;
    public UXButton reviveForVideoButton;
    public UXButton reviveForCoinsButton;
    public Image reviveFillImage;
    public Animator gameOverPanelAnimator;
    public ScoreAnimationController scoreAnimation;

    [Space(5)]
    public Text highScoreText;
    public Text highScoreLableText;
    public Text finalScoreText;
    public Text placeText;

    [Space(5)]
    public MultilanguageTextProperties highscoreMultilanguageTextProp;

    private Coroutine coinsPanelFadeCoroutine;
    private GameController gameController;

    private bool reviveAvailable;
    private bool reviveSkiped;

    private int openRevivePanelParam;
    private int openGameOverPanelParam;
    private int onReviveSkipedParam;

    private const string PREF_KEY = "TotalScore";
    [Header("UI hiển thị điểm tổng")]
    public Text scoreText;


    private void Awake()
    {
        instance = this;
        
        gameController = ReferenceController.instance.gameController;

        openRevivePanelParam = Animator.StringToHash("openRevivePanel");
        openGameOverPanelParam = Animator.StringToHash("openGameOverPanel");
        onReviveSkipedParam = Animator.StringToHash("onReviveSkiped");
    }
    
    public void ShowGameUI()
    {
        gamePanel.SetActive(true);
        ShowScoreText();
    }

    public void UpdateScoreText(int newScore, int addedPoints)
    {
        scoreAnimation.UpdateScore(newScore, addedPoints);
    }

    public void HideScoreText()
    {
        scoreAnimation.HideScoreText();
    }

    public void ShowScoreText()
    {
        scoreAnimation.ShowScoreText();
    }

    public void ShowGameOverPanel(int finalScore, int highScore, bool newHighScore, bool enableRevive)
    {
        gameController.EnableLowpassMusicEffect();
        DisplayGameOverPanel(finalScore, highScore, newHighScore, enableRevive);
    }

    private void UpdateTotalScore(int finalScore)
    {
        // 🟢 Cộng điểm mới vào điểm đã lưu
        int current = PlayerPrefs.GetInt(PREF_KEY, 0);
        int newTotal = current + finalScore;

        // 🟢 Lưu lại
        PlayerPrefs.SetInt(PREF_KEY, newTotal);
        PlayerPrefs.Save();

        // 🟢 Debug log
        Debug.Log($"✅ finalScore: {finalScore}, TotalScore: {newTotal}");

        // 🟢 Cập nhật UI
        if (scoreText != null)
        {
            scoreText.text = newTotal.ToString();
        }
        else
        {
            Debug.LogWarning("❗ scoreText chưa được gán.");
        }
    }

    private void DisplayGameOverPanel(int finalScore, int highScore, bool newHighScore, bool enableRevive)
    {
        if (newHighScore)
        {
            highscoreMultilanguageTextProp.InitWord("gameplay.highscore.new");
        }
        else
        {
            highscoreMultilanguageTextProp.InitWord("gameplay.highscore");
        }

        highScoreText.text = highScore.ToString();

        finalScoreText.text = finalScore.ToString();

        Debug.Log("finalScore" + finalScore);
        UpdateTotalScore(finalScore);

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);

        // checking for revive availability
        reviveForVideoButton.Interactable = IsRewardBasedVideoReady();
        
        if (enableRevive && reviveForVideoButton.Interactable != false)
        {
            gameOverPanelAnimator.SetTrigger(openRevivePanelParam);

            StartCoroutine(RevivePanelAnimation());
        }
        else
        {
            gameOverPanelAnimator.SetTrigger(openGameOverPanelParam);
            gameController.OnGameCompleted();
        }
    }

    private bool IsRewardBasedVideoReady()
    {
        return AdsManager.IsRewardBasedVideoLoaded(AdsManager.Settings.RewardedVideoType);
    }

    private IEnumerator RevivePanelAnimation()
    {
        // wait for UI animation finished
        yield return new WaitForSeconds(1.5f);

        float timer = reviveWaitTime;
        reviveAvailable = true;
        reviveSkiped = false;

        while (reviveAvailable && !reviveSkiped && timer > 0)
        {
            reviveFillImage.fillAmount = timer / reviveWaitTime;

            timer -= Time.deltaTime;
            yield return null;
        }

        // if revive available than time passed and buttons was not pressed
        if (reviveAvailable)
        {
            reviveAvailable = false;
            reviveSkiped = true;
        }

        if (reviveSkiped)
        {
            gameController.OnGameCompleted();
        }

        gameOverPanelAnimator.SetTrigger(onReviveSkipedParam);
    }

    private void RevivePlayer()
    {
        reviveAvailable = false;

        gameController.RevivePlayer();
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameController.DisableLowpassMusicEffect();
    }

    public void HomeButton()
    {
        GameController.RestartGame();
    }

    public void ReplayButton()
    {
        GameController.RestartGame(false);
    }

    public void ShareButton()
    {
        ShareController.ShareMessage();
    }

    //public void ReviveForVideo()
    //{
    //    if (reviveAvailable)
    //    {
    //        RevivePlayer();
    //    }
    //}

    public void ReviveForVideo()
    {
        if (!reviveAvailable)
            return;

        int revivalToken = PlayerPrefs.GetInt("RevivalTokenCount", 0);

        if (revivalToken >= 1)
        {
            revivalToken -= 1;
            PlayerPrefs.SetInt("RevivalTokenCount", revivalToken);
            PlayerPrefs.Save(); // đảm bảo lưu ngay
            Debug.Log($"🔁 Reviving player - remaining GEM: {PlayerPrefs.GetInt("RevivalTokenCount", 0)}");
            RevivePlayer();

            // Gợi ý: nếu bạn có UI hiển thị GEM, thì gọi hàm cập nhật lại ở đây
            //FindObjectOfType<TokenDisplayUI>()?.UpdateTokenDisplay();
        }
        else
        {
            Debug.LogWarning("❌ Not enough GEM to revive.");
            // Có thể hiển thị popup hoặc statusText tại đây nếu muốn
        }
    }

    public void SkipRevive()
    {
        reviveSkiped = true;
    }
}