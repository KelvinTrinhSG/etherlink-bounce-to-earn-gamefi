#pragma warning disable 0414

using System;
using System.Collections;
using UnityEngine;
using Watermelon;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    
    public GameSettings gameSettings;

    [Space]
    public bool isGameActive = false;

    public int minDistanceForRevive = 50;
    public float relaxZoneHeight = 150f;
    public int relaxZoneWidth = 17;

    [Header("Easter eggs")]
    [Range(0, 1)]
    public float wilhelmScreamChance = 0.01f;

    private ReferenceController referenceController;
    private SpawnController spawnController;
    private UIControllerGame uiController;
    private Transform playerTransform;
    private PlayerController playerController;
    private Pool scorePointersPool;
    private AudioCaseCustom[] customMusicCases = new AudioCaseCustom[2];
    private AudioClip gameOverSound;
    private ScorePointer playerHighscorePointer;
    private StoreProduct currentCharacter;

    private DeathReason lastDeathReson;

    private bool isReviveUsed = false;
    private bool skipedFirstHitScoreIncreasing = false;
    private int score = 0;

    private static float lastAddsTime = float.MinValue;
    private static int playedGames = 0;

    public static bool showMenu = true;

    public static bool IsGameActive { get { return instance.isGameActive; } }
    public int CurrentScore { get { return score; } }
    public StoreProduct CurentCharacter { get { return currentCharacter; } set { currentCharacter = value; } }
    
    private void Awake()
    {
        instance = this;
        referenceController = ReferenceController.instance;

        spawnController = referenceController.spawnController;
        uiController = referenceController.uiController;
        playerTransform = referenceController.playerController.transform;
        playerController = referenceController.playerController;

        Time.timeScale = 1f;
        MusicContoroll();

        minDistanceForRevive = gameSettings.reviveMinDistance;
    }

    public static void TryIntrastitial()
    {
        if (playedGames % instance.gameSettings.showInterstitialEveryGame == 0 && lastAddsTime + instance.gameSettings.showInterstitialDelay < Time.realtimeSinceStartup)
        {
            AdsManager.ShowInterstitial(delegate { });

            lastAddsTime = Time.realtimeSinceStartup;
        }
    }

    public static void StartGame()
    {
        instance.StartGameProcess();
    }

    public void Start()
    {
        currentCharacter = StoreController.GetProduct(PlayerPrefs.GetInt("LAST_SKIN_ID", 0));
        playerController.InitiazeSkin(currentCharacter, true);
        gameOverSound = currentCharacter.gameOverSound != null ? currentCharacter.gameOverSound : AudioController.Sounds.gameOverSound;

        // Infinite mode initialization
        playedGames++;
        spawnController.LaunchOnInfiniteMode();
        score = 0;

        lastDeathReson = DeathReason.Falling;

        scorePointersPool = PoolManager.GetPoolByName("ScorePointer");

        int highScore = PrefsSettings.GetInt(PrefsSettings.Key.HighScore);

        if (highScore > 2)
        {
            playerHighscorePointer = scorePointersPool.GetPooledObject(true).GetComponent<ScorePointer>();
            playerHighscorePointer.Initialize("you", highScore);
        }
    }

    [ContextMenu("Start Game")]
    private void StartGameProcess()
    {
        isGameActive = true;
        uiController.ShowGameUI();
        playerController.SimulateFirstTap();
        CameraController.OnGameStarted();
    }

    private void OnEnable()
    {
        PauseManager.IsAutoUnpauseAllowed += IsPauseAllowed;
    }

    private void OnIntrastitialClosed()
    {
        PauseManager.SetPause(false);
    }

    private void OnDisable()
    {
        PauseManager.IsAutoUnpauseAllowed -= IsPauseAllowed;
    }

    public static void RestartGame(bool showMenuAfterRestart = true)
    {
        showMenu = showMenuAfterRestart;

        TryIntrastitial();

        AudioController.ReleaseStreams();

        SceneLoader.LoadScene("Game", SceneLoader.SceneTransition.Fade);
    }

    public void GameOver(DeathReason deathReason)
    {
        if (UnityEngine.Random.Range(0f, 1f) <= wilhelmScreamChance)
        {
            AudioController.PlaySound(AudioController.Sounds.wilhelmScream);
        }
        else
        {
            AudioController.PlaySound(gameOverSound);
        }

        lastDeathReson = deathReason;
        isGameActive = false;

        ShowGameOverPanel();

        AdsManager.HideBanner(AdsManager.Settings.BannerType);
    }

    public void RevivePlayer()
    {
        isReviveUsed = true;

        //AdsManager.ShowRewardBasedVideo(AdsManager.Settings.RewardedVideoType, (hasReward) =>
        //{
        //    if (hasReward)
        //    {
        //        AdsManager.ShowBanner(AdsManager.Settings.BannerType);

        //        playerController.Revive(new Vector3(spawnController.SpawnRevivePlatform(), 2f));
        //    }
        //    else
        //    {
        //        StartCoroutine(WaitForFrame(ShowGameOverPanel));
        //    }
        //});

        playerController.Revive(new Vector3(spawnController.SpawnRevivePlatform(), 2f));

        AchievementManager.IncrementProgress(12, 1); //Revive 5 times
        AchievementManager.IncrementProgress(13, 1); //Revive 20 times
    }

    private IEnumerator WaitForFrame(Action callback)
    {
        yield return null;

        if (callback != null)
        {
            callback.Invoke();
        }
    }

    public void OnPlayerRevived()
    {
        isGameActive = true;
        uiController.ShowGameUI();
    }

    public void AddScorePoints(int pointsAmount)
    {
        if (!isGameActive)
            return;

        if (skipedFirstHitScoreIncreasing)
        {
            score += pointsAmount;
        }
        else
        {
            skipedFirstHitScoreIncreasing = true;
        }

        uiController.UpdateScoreText(score, pointsAmount);
    }

    private void ShowGameOverPanel()
    {
        int finalScore = score;
        int highScore = PrefsSettings.GetInt(PrefsSettings.Key.HighScore);
        bool newHighScore = false;

        if (highScore < finalScore)
        {
            highScore = score;
            PrefsSettings.SetInt(PrefsSettings.Key.HighScore, highScore);
            newHighScore = true;
        }

        bool enableRevive = !isReviveUsed && PlayerController.Position.x >= minDistanceForRevive;

        uiController.ShowGameOverPanel(finalScore, highScore, newHighScore, enableRevive);

        SendAchievementsResults(finalScore);
    }

    public void OnGameCompleted()
    {
        AchievementManager.IncrementProgress(1, 1); //Play 10 games
        AchievementManager.IncrementProgress(2, 1); //Play 50 games
    }

    private void SendAchievementsResults(int finalScore)
    {
        playerController.SendAchievementsResults();

        AchievementManager.SetProgress(5, finalScore); //Score 50 points
        AchievementManager.SetProgress(6, finalScore); //Score 200 points

        if (finalScore == 0)
        {
            AchievementManager.UnlockAchievement(14); //Score 0 points
        }

        StoreController.PlaySkin(currentCharacter.ID);
    }

    public bool IsPauseAllowed()
    {
        return !isGameActive;
    }

    public static void DisplayFriendsHighScore(int score, string nickName)
    {
        instance.scorePointersPool.GetPooledObject(true).GetComponent<ScorePointer>().Initialize(nickName, score);
    }

    private void MusicContoroll()
    {
        //double initLatency = 0.115f;
        //double playTime = 0f; 

        referenceController.musicEffectsMixer.SetFloat("lowpassParam", 10000f);

        customMusicCases[0] = AudioController.GetCustomSource(true, AudioController.AudioType.Music);
        //customMusicCases[1] = AudioController.GetCustomSource(true, AudioController.AudioType.Music);

        customMusicCases[0].source.outputAudioMixerGroup = referenceController.musicEffectsMixer.FindMatchingGroups("Master")[0];
        //customMusicCases[1].source.outputAudioMixerGroup = referenceController.musicEffectsMixer.FindMatchingGroups("Master")[0];

        customMusicCases[0].source.clip = AudioController.MusicAudioClips.GetRandomItem();
        //customMusicCases[1].source.clip = AudioController.Settings.mainThemeLoopSound;

        customMusicCases[0].source.Play();
        customMusicCases[0].source.loop = false;

        //playTime += (double)AudioController.Settings.mainThemeLoopSound.length - initLatency;

        //customMusicCases[1].source.loop = true;
        //customMusicCases[1].source.PlayDelayed((float)playTime);
    }

    public void EnableLowpassMusicEffect()
    {
        Tween.DoFloat(10000f, 450f, 0.3f, (float res) => referenceController.musicEffectsMixer.SetFloat("lowpassParam", res), true);
    }

    public void DisableLowpassMusicEffect()
    {
        Tween.DoFloat(450f, 10000f, 0.3f, (float res) => referenceController.musicEffectsMixer.SetFloat("lowpassParam", res), true);
    }

    public void RunHighscorePointerAnimation()
    {
        playerHighscorePointer.RunPassedAnimation();
    }
}