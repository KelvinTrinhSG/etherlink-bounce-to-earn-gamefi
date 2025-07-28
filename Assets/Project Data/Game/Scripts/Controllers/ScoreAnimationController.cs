using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public class ScoreAnimationController : MonoBehaviour
{
    public Text scoreText;
    public RectTransform scoreTextRect;

    public Text additionalScoreText1;
    public Text additionalScoreText2;

    public float appearingTime = 0.25f;
    public float waitTime = 0.5f;
    public float hideTime = 0.12f;
    public float punchScaleTime = 0.2f;

    private List<NewScore> queue = new List<NewScore>();
    private Text currentText;
    private RectTransform currentTextRect;

    private bool isAnimationActive;
    private bool isSleeping;
    private bool objectsToggle;

    private float timer = 0f;

    [System.Serializable]
    private struct NewScore
    {
        public int newScore;
        public int additionalPoints;

        public NewScore(int score, int addition)
        {
            newScore = score;
            additionalPoints = addition;
        }
    }

    public void ShowScoreText()
    {
        scoreText.DOFade(1, 0.7f).SetEasing(Ease.Type.QuartIn);
        isAnimationActive = true;
        StartCoroutine(AnimationUpdate());
    }

    public void UpdateScore(int newScore, int additionalPoints)
    {
        if (isAnimationActive)
        {
            queue.Add(new NewScore(newScore, additionalPoints));
            timer = 0f;
        }
    }

    public void HideScoreText()
    {
        scoreText.DOFade(0, 2f).SetEasing(Ease.Type.QuartIn);
        isAnimationActive = false;
    }

    private IEnumerator AnimationUpdate()
    {
        while (isAnimationActive)
        {

            while (queue.Count == 0)
            {
                yield return null;
            }

            scoreText.text = queue[0].newScore.ToString();
            scoreTextRect.DOScale(1.15f, punchScaleTime * 0.3f).SetEasing(Ease.Type.CubicIn).OnComplete(() => scoreTextRect.DOScale(1f, punchScaleTime * 0.7f).SetEasing(Ease.Type.CubicOut));

            if (queue[0].additionalPoints > 1)
            {
                InitCurrentAdditionalText();

                //currentText.DOFade(0.65f, appearingTime * 0.8f).SetEasing(Ease.Type.CubicIn);
                //yield return new WaitForSeconds(appearingTime * 0.8f);

                timer = queue.Count == 1 ? waitTime : 0f;

                while (timer > 0)
                {
                    timer -= Time.deltaTime;
                    yield return null;
                }

                currentText.DOFade(0f, hideTime).SetEasing(Ease.Type.CubicIn);
                yield return new WaitForSeconds(hideTime);
            }

            queue.RemoveAt(0);
        }
    }

    private void InitCurrentAdditionalText()
    {
        objectsToggle = !objectsToggle;

        currentText = objectsToggle ? additionalScoreText1 : additionalScoreText2;

        currentText.text = "+" + queue[0].additionalPoints;
        currentText.color = currentText.color.SetAlpha(0.65f);
    }


    private int devScore = 0;

    [Button("Add score")]
    public void AddScore()
    {
        int addition = Random.Range(1, 4);
        devScore += addition;
        UpdateScore(devScore, addition);
    }
}