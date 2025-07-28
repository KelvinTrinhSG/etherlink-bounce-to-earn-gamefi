#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;

namespace Watermelon
{
    public class AchievementPanel : MonoBehaviour
    {
        [SerializeField]
        private RectTransform container;
        private CanvasGroup containerCanvasGroup;

        [SerializeField]
        private Text achievementText;
        //[SerializeField] private Image achievementIcon; // Archevement panel don`t contain icon

        [Space]
        [SerializeField]
        [MultilanguageWord("ui.achievements")]
        private string defaultAchievementTitleKey = "ui.achievements.panel.title";
        private string defaultAchievementTitle;

        private TweenCase tween;

        private void Awake()
        {
            containerCanvasGroup = container.GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            InitLangugage();

            AchievementManager.OnAchievementUnlocked += OnAchievementUnlock;

            Multilanguage.OnLanguageChanged += InitLangugage;
        }

        private void OnSceneChanged()
        {
            containerCanvasGroup.alpha = 0;
        }

        private void InitLangugage()
        {
            defaultAchievementTitle = Multilanguage.GetWord(defaultAchievementTitleKey);

            Font languageFont = Multilanguage.GetFont("default");
            achievementText.font = languageFont;
        }

        private void OnAchievementUnlock(Achievement achievement)
        {
            if (tween != null && !tween.isCompleted)
                tween.Kill();

            achievementText.text = defaultAchievementTitle;
            // the  code below uses achievementIcon ref
            //if (achievement.Icon != null) 
            //{
            //    achievementIcon.gameObject.SetActive(true);
            //    achievementIcon.sprite = achievement.Icon;
            //}
            //else
            //{
            //    achievementIcon.gameObject.SetActive(false);
            //}

            tween = containerCanvasGroup.DOFade(1, 1f).OnComplete(delegate
            {
                tween = containerCanvasGroup.DOFade(0, 0.5f);
            });
        }

        private void OnAchievementMessage(string title, string content, Sprite icon)
        {
            if (tween != null && !tween.isCompleted)
                tween.Kill();

            achievementText.text = content;

            // the  code below uses achievementIcon ref
            //if (icon != null)
            //{
            //    achievementIcon.gameObject.SetActive(true);
            //    achievementIcon.sprite = icon;
            //}
            //else
            //{
            //    achievementIcon.gameObject.SetActive(false);
            //}

            container.sizeDelta = new Vector2(achievementText.preferredWidth + 300, container.sizeDelta.y);

            tween = containerCanvasGroup.DOFade(1, 1f).OnComplete(delegate
            {
                tween = containerCanvasGroup.DOFade(0, 0.5f);
            });
        }
    }
}