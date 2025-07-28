#pragma warning disable 0649

using UnityEngine;
using UnityEngine.UI;
using Watermelon;

public sealed class PageMain : Page
{    
    [Space]
    [SerializeField]
    private PageController pageController;

    [Space]
    [SerializeField] GameObject settingsButton;
    
    [Space]
    [SerializeField]
    private Animator topToPlayAnimator;

    [Space]
    [SerializeField]
    private GameObject tapToPlayZone;

    [Space]
    [SerializeField]
    private GameObject worldMenuCanvas;
    [SerializeField]
    private Transform storeButtonTransform;

    private void Awake()
    {

    }

    public override void Close(bool immediately)
    {
        settingsButton.gameObject.SetActive(false);

        topToPlayAnimator.gameObject.SetActive(false);
        topToPlayAnimator.enabled = false;

        tapToPlayZone.SetActive(false);

        if(immediately)
        {
            storeButtonTransform.gameObject.SetActive(false);
        }
        else
        {
            storeButtonTransform.DOScaleY(0f, 0.2f).SetEasing(Ease.Type.CubicIn);
        }

        worldMenuCanvas.SetActive(true);
    }

    public override void Open()
    {
        Tween.DelayedCall(0.4f, () => settingsButton.gameObject.SetActive(true));

        storeButtonTransform.DOScaleY(1f, 0.2f).SetEasing(Ease.Type.CubicIn);

        worldMenuCanvas.SetActive(true);
    }

    public override void OnClosed()
    {
        Debug.Log("OnClosed");
    }

    public override void OnOpened()
    {
        topToPlayAnimator.gameObject.SetActive(true);
        topToPlayAnimator.enabled = true;

        tapToPlayZone.SetActive(true);
    }

#region Buttons
    public void PlayButton()
    {
        //SceneLoader.LoadScene("Game");
    }

    public void StoreButton()
    {
        PageController.Open(PageType.Store);
    }
#endregion
}
