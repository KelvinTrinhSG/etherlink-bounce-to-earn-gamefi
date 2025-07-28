using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Watermelon;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Text))]
public class FloatingText : MonoBehaviour
{
    public static FloatingText instance;
    public static bool hideNormalText;

    [Header("Settings")]
    public float movingSpeed = 100f;
    public float visibleTime = 1f;

    [Header("References")]
    public Text textRef;
    public ComboVisualizer comboVisualizer;
    //public Outline outlineRef;

    [Header("States settings")]
    public TextLookSettings normalLook;
    public TextLookSettings basicStateLook;
    public TextLookSettings middleStateLook;
    public TextLookSettings ultraStateLook;
    public TextLookSettings comboBreakLook;

    private Vector2 startPosition;
    private Coroutine currentCoroutine;
    private TextStyle currentStyle;

    private static bool reinitStyle;
    private bool axcelerateFade;

    private float outlineNormalAlpha;
    private float vertDirectionCoef;

    [System.Serializable]
    public struct TextLookSettings
    {
        public int fontSize;
        [Range(-1, 1)]
        public float vertMoveCoef;
        public float outlineSize;
        public Color textColor;
    }

    public enum TextStyle
    {
        Normal,
        BasicState,
        MiddleState,
        UltraState,
        ComboBreak,
    }

    public void Awake()
    {
        instance = this;

        textRef.color = textRef.color.SetAlpha(0);
        startPosition = textRef.rectTransform.anchoredPosition;

        InitStyle(TextStyle.Normal);
    }

    public void Init(string textToDisplay, TextStyle style, float movingSpeed = 100f, float visibleTime = 1f)
    {
        axcelerateFade = false;
        gameObject.SetActive(true);
        textRef.text = textToDisplay;

        if (currentStyle != style || reinitStyle)
        {
            reinitStyle = false;
            InitStyle(style);
        }

        InitColor();

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }


        if (style == TextStyle.Normal || style == TextStyle.ComboBreak)
        {
            if (!hideNormalText && gameObject.activeInHierarchy)
            {
                currentCoroutine = StartCoroutine(FloatCoroutine());
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            reinitStyle = true;
            ScaledAppearing();
        }
    }

    private void InitStyle(TextStyle style)
    {
        currentStyle = style;
        TextLookSettings lookSettings = GetLookSettings(style);

        vertDirectionCoef = lookSettings.vertMoveCoef;

        textRef.fontSize = lookSettings.fontSize;
    }

    private void InitColor()
    {
        textRef.color = ComboVisualizer.GetStateColor(PlayerController.ComboState);//lookSettings.textColor;
    }

    private TextLookSettings GetLookSettings(TextStyle style)
    {
        if (style == TextStyle.Normal)
        {
            return normalLook;
        }
        else if (style == TextStyle.BasicState)
        {
            return basicStateLook;
        }
        else if (style == TextStyle.MiddleState)
        {
            return middleStateLook;
        }
        else if (style == TextStyle.UltraState)
        {
            return ultraStateLook;
        }
        else
        {
            return comboBreakLook;
        }
    }

    private IEnumerator FloatCoroutine()
    {
        textRef.rectTransform.anchoredPosition = startPosition;
        float timer = visibleTime;// * (currentStyle == TextStyle.ComboBreak ? 0.5f : 1f);
        float startAlpha = textRef.color.a;

        while (timer > 0)
        {
            textRef.rectTransform.anchoredPosition += new Vector2(0f, vertDirectionCoef * movingSpeed * Time.deltaTime);

            textRef.color = textRef.color.SetAlpha(timer / visibleTime * startAlpha);
            //outlineRef.effectColor = outlineRef.effectColor.SetAlpha(timer / visibleTime * outlineNormalAlpha);

            timer -= Time.deltaTime * (axcelerateFade ? 4f : 1f);
            yield return null;
        }

        textRef.color = textRef.color.SetAlpha(startAlpha);
        gameObject.SetActive(false);
    }

    private void ScaledAppearing(/*System.Action onTextAppeared*/)
    {
        textRef.rectTransform.localScale = new Vector3();
        textRef.rectTransform.eulerAngles = new Vector3(0f, 0f, Random.Range(-12f, 12f));
        hideNormalText = true;

        textRef.rectTransform.DOScale(1.2f, 0.5f).SetEasing(Ease.Type.CircIn).OnComplete(() =>
        {
            //onTextAppeared?.Invoke();

            textRef.rectTransform.DOScale(1f, 0.2f).SetEasing(Ease.Type.CircOut).OnComplete(() =>
            {
                hideNormalText = false;
                textRef.DOFade(0f, 0.3f).SetEasing(Ease.Type.CircIn).OnComplete(() =>
                {
                    gameObject.SetActive(false);
                });
            });
        });
    }

    public void AxcelerateFade()
    {
        axcelerateFade = true;
    }

}